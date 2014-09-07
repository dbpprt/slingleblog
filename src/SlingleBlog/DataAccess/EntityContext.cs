using FileBiggy;
using SlingleBlog.Models;

namespace SlingleBlog.DataAccess
{
    public class EntityContext : BiggyContext
    {
        public EntityContext(string connectionString) 
            : base(connectionString)
        {
        }

        public EntitySet<Post> Posts { get; set; }

        public EntitySet<RegisteredJob> Jobs { get; set; }

        public EntitySet<ScheduledJobExecution> ScheduledJobExecutions { get; set; }

        public EntitySet<JobHistoryItem> JobHistory { get; set; }

        public EntitySet<PrecompiledPage> PrecompiledPages { get; set; } 
    }
}
