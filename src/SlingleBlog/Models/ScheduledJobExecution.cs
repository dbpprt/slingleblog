using System;
using MobileDB.Common.Attributes;

namespace SlingleBlog.Models
{
    public class ScheduledJobExecution
    {
        [Identity]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public DateTime ExecuteAt { get; set; }

    }
}
