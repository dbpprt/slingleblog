namespace SlingleBlog.Common.Configuration
{
    public interface IAccount
    {
        string DisplayName { get; }

        string Username { get; }

        string Password { get; }

        string[] Roles { get; }
    }
}
