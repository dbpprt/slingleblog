using System;
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

        public static void Merge(string sourceDirectry, string targetDirectory)
        {
            var source = new DirectoryInfo(sourceDirectry);
            var target = new DirectoryInfo(targetDirectory);

            if (String.Equals(source.FullName, target.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.ToString(), file.Name), true);
            }

            foreach (var subDir in source.GetDirectories())
            {
                var nextSubDir =
                    target.CreateSubdirectory(subDir.Name);
                Merge(subDir.FullName, nextSubDir.FullName);
            }
        }
    }
}
