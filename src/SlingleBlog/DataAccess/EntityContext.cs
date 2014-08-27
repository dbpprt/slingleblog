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
    }
}
