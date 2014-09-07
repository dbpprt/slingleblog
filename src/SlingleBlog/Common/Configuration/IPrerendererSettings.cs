namespace SlingleBlog.Common.Configuration
{
    public interface IPrerendererSettings
    {
        int RecompileAfterHours { get; }

        string DomVariable { get; }

        int Timeout { get; }
    }
}
