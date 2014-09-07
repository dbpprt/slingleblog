using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FileBiggy.Contracts;
using Microsoft.Practices.Unity;
using SlingleBlog.Models;

namespace SlingleBlog.Common.Scheduler
{
    public class JobScheduler : IDisposable
    {
        private readonly IEntitySet<RegisteredJob> _registeredJobs;
        private readonly IEntitySet<ScheduledJobExecution> _scheduledJobExecutions;
        private readonly IUnityContainer _container;
        private readonly List<Job> _jobs;
        private Timer _timer;
        private readonly CancellationTokenSource _cancellationToken;

        public JobScheduler(
            IEntitySet<RegisteredJob> registeredJobs,
            IEntitySet<ScheduledJobExecution> scheduledJobExecutions,
            IUnityContainer container
            )
        {
            _registeredJobs = registeredJobs;
            _scheduledJobExecutions = scheduledJobExecutions;
            _container = container;
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
                .Where(_ => _scheduledJobExecutions.All(execution => execution.JobId != _.Id));

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
        }

        private void SynchronizeRegisteredJobs()
        {
            // the first step is to synchronize all jobs with our database
            foreach (var job in _jobs)
            {
                var existing = _registeredJobs.FirstOrDefault(_ => _.Id == job.Id);

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
        }

        private void OnTick(object state)
        {
            Pause();
            // todo: do we need a lock here?

            var pendingExecutions = _scheduledJobExecutions.Where(_ => _.ExecuteAt < DateTime.UtcNow);

            foreach (var scheduledJobExecution in pendingExecutions)
            {
                _scheduledJobExecutions.Remove(scheduledJobExecution);

                var job = _jobs.FirstOrDefault(_ => _.Id == scheduledJobExecution.JobId);

                if (job == null)
                {
                    // TODO: Logging
                    continue;
                }

                var scope = _container.CreateChildContainer();
                var task = job.Execute(_cancellationToken.Token, scope);

                task.ContinueWith(o =>
                {
                    scope.Dispose();
                    _scheduledJobExecutions.Add(new ScheduledJobExecution
                    {
                        Id = Guid.NewGuid(),
                        ExecuteAt = DateTime.UtcNow.Add(job.RunEvery),
                        JobId = job.Id
                    });
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
