using System.Collections.Generic;

namespace SlingleBlog.Common.Configuration
{
    public interface IConfiguration
    {
        string BasePath { get; }

        string StorageBasePath { get; }

        string PublicPath { get; }

        bool UseErrorPage { get; }

        List<string> DefaultFiles { get; }

        string ApiErrorPolicy { get; }
    }
}