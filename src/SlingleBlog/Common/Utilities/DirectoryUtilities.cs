using System.IO;

namespace SlingleBlog.Common.Utilities
{
    public static class DirectoryUtilities
    {
        public static string EnsureExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
