using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FileBiggy.Contracts;
using SlingleBlog.Models;
using SlingleBlog.ViewModels;

namespace SlingleBlog.Controllers
{
    public class JobController : ApiController
    {
        private readonly IEntitySet<ScheduledJobExecution> _scheduledJobExecutions;
        private readonly IEntitySet<RegisteredJob> _jobs;
        private readonly IEntitySet<JobHistoryItem> _jobHistory;

        public JobController(
            IEntitySet<ScheduledJobExecution> scheduledJobExecutions,
            IEntitySet<RegisteredJob> jobs,
            IEntitySet<JobHistoryItem> jobHistory
            )
        {
            _scheduledJobExecutions = scheduledJobExecutions;
            _jobs = jobs;
            _jobHistory = jobHistory;
        }

        [HttpGet]
        [Route("api/sys/jobs")]
        public IHttpActionResult Jobs()
        {
            var jobs = _jobs.ToList();
            var scheduledExecutions = _scheduledJobExecutions.ToList();

            var result = (from job in jobs
                          let scheduled = scheduledExecutions.FirstOrDefault(_ => _.JobId == job.Id)
                          let nextScheduledExecution = scheduled != null ? scheduled.ExecuteAt : DateTime.MinValue
                          select new JobViewModel
                          {
                              JobId = job.Id,
                              JobName = job.JobName,
                              Description = job.Description,
                              Interval = job.RunEvery,
                              NextScheduledExecution = nextScheduledExecution
                          }).ToList();

            return Ok(result);
        }
    }
}
