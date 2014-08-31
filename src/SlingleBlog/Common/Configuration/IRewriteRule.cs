namespace SlingleBlog.Common.Configuration
{
    public interface IRewriteRule
    {
        string Name { get; set; }

        string Constraint { get; set; }

        string Template { get; }

        string RewriteTo { get; set; }
    }
}
