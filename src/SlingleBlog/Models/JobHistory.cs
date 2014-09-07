using System;
using FileBiggy.Attributes;

namespace SlingleBlog.Models
{
    public class JobHistoryItem
    {
        [Identity]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public DateTime ExecutionStartTime { get; set; }

        public DateTime ExecutionEndTime { get; set; }
    }
}
