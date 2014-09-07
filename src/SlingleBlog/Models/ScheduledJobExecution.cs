using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileBiggy.Attributes;

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
