using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using MobileDB.Contracts;
using SlingleBlog.Common.Utilities;
using SlingleBlog.Models;

namespace SlingleBlog.Common.Scheduler
{
    public class JobScheduler : IDisposable
    {
        private readonly IEntitySet<RegisteredJob> _registeredJobs;
        private readonly IEntitySet<ScheduledJobExecution> _scheduledJobExecutions;
        private readonly IUnityContainer _container;
        private readonly IDbContext _context;
        private readonly List<Job> _jobs;
        private Timer _timer;
        private readonly CancellationTokenSource _cancellationToken;

        public JobScheduler(
            IEntitySet<RegisteredJob> registeredJobs,
            IEntitySet<ScheduledJobExecution> scheduledJobExecutions,
            IUnityContainer container,
            IDbContext context
            )
        {
            _registeredJobs = registeredJobs;
            _scheduledJobExecutions = scheduledJobExecutions;
            _container = container;
            _context = context;
            _jobs = new List<Job>();
            _cancellationToken = new CancellationTokenSource();

            _jobs = container.ResolveAll<Job>().ToList();

            SynchronizeRegisteredJobs();

            SynchronizePendingJobExecutions();
        }

        public void Start()
        {
            _timer = new Timer(OnTick, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        public void Resume()
        {
            _timer.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        public void Pause()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SynchronizePendingJobExecutions()
        {
            // we need to seed job executions if jobs are newly created
            var pendingInserts = _jobs
                .Where(_ => _scheduledJobExecutions.AsQueryable().All(execution => execution.JobId != _.Id));

            foreach (var pendingInsert in pendingInserts)
            {
                var entity = new ScheduledJobExecution
                {
                    ExecuteAt = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    JobId = pendingInsert.Id
                };

                _scheduledJobExecutions.Add(entity);
            }

            _context.SaveChanges();
        }

        private void SynchronizeRegisteredJobs()
        {
            // the first step is to synchronize all jobs with our database
            foreach (var job in _jobs)
            {
                var existing = _registeredJobs.AsQueryable().FirstOrDefault(_ => _.Id == job.Id);

                if (existing == null)
                {
                    var entity = new RegisteredJob
                    {
                        Description = job.Description,
                        Id = job.Id,
                        JobName = job.JobName,
                        RunEvery = job.RunEvery
                    };

                    _registeredJobs.Add(entity);
                }
                else
                {
                    if (existing.Description != job.Description ||
                        existing.JobName != job.JobName)
                    {
                        existing.Description = job.Description;
                        existing.JobName = job.JobName;

                        _registeredJobs.Update(existing);
                    }
                }
            }

            _context.SaveChanges();
        }

        private void OnTick(object state)
        {
            Pause();
            // todo: do we need a lock here?

            var contextLock = new ReaderWriterLockSlim();

            var pendingExecutions = _scheduledJobExecutions.AsQueryable().Where(_ => _.ExecuteAt < DateTime.UtcNow);

            foreach (var scheduledJobExecution in pendingExecutions)
            {
                Job job;

                using (contextLock.WriteLock())
                {
                    _scheduledJobExecutions.Remove(scheduledJobExecution);
                    _context.SaveChanges();

                    job = _jobs.FirstOrDefault(_ => _.Id == scheduledJobExecution.JobId);

                    if (job == null)
                    {
                        // TODO: Logging
                        continue;
                    }
                }
                var scope = _container.CreateChildContainer();
                var task = job.Execute(_cancellationToken.Token, scope);

                task.ContinueWith(o =>
                {
                    scope.Dispose();
                    using (contextLock.WriteLock())
                    {
                        _scheduledJobExecutions.Add(new ScheduledJobExecution
                        {
                            Id = Guid.NewGuid(),
                            ExecuteAt = DateTime.UtcNow.Add(job.RunEvery),
                            JobId = job.Id
                        });

                        _context.SaveChanges();
                    }
                });
            }

            Resume();
        }

        public void Dispose()
        {
            _timer.Dispose();
            _cancellationToken.Cancel();
        }
    }
}
