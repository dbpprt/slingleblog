using System;
using System.Linq;
using SlingleBlog.Common.Utilities;

namespace SlingleBlog.Models
{
    static class Extensions
    {

        public static Post RenderPreviewMarkup(this Post post)
        {
            const string delimiter = "<!--- end-preview -->";
            var preview = post.Content.Split(new[] {delimiter}, StringSplitOptions.RemoveEmptyEntries).First();
            post.Content = new Markdown().Transform(preview);
            return post;
        }

        public static Post RenderMarkup(this Post post)
        {
            post.Content = new Markdown().Transform(post.Content);
            return post;
        }
    }
}
