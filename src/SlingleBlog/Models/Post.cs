using System;
using FileBiggy.Attributes;

namespace SlingleBlog.Models
{
    public class Post
    {
        [Identity]
        public string Slug { get; set; }

        public string Content { get; set; }

        public DateTime PubDate { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsPublished { get; set; }

        public string[] Tags { get; set; }

        public string Title { get; set; }
    }
}
