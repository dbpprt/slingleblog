using MobileDB;
using MobileDB.Contracts;
using SlingleBlog.Models;

namespace SlingleBlog.DataAccess
{
    public class EntityContext : DbContext
    {
        public EntityContext(string connectionString) 
            : base(connectionString)
        {
        }

        public IEntitySet<Post> Posts { get; set; }

        public IEntitySet<RegisteredJob> Jobs { get; set; }

        public IEntitySet<ScheduledJobExecution> ScheduledJobExecutions { get; set; }

        public IEntitySet<JobHistoryItem> JobHistory { get; set; }

        public IEntitySet<PrecompiledPage> PrecompiledPages { get; set; } 
    }
}
