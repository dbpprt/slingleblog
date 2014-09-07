using System;
using System.Linq;
using SlingleBlog.Common.Utilities;

namespace SlingleBlog.Models
{
    static class Extensions
    {

        public static string RenderPreviewMarkup(this Post post)
        {
            const string delimiter = "<!--- end-preview -->";
            var preview = post.Content.Split(new[] {delimiter}, StringSplitOptions.RemoveEmptyEntries).First();
            return new Markdown().Transform(preview);;
        }

        public static string RenderMarkup(this Post post)
        {
            return new Markdown().Transform(post.Content);
        }
    }
}
