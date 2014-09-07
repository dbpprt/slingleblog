using System;
using FileBiggy.Attributes;

namespace SlingleBlog.Models
{
    public class RegisteredJob
    {
        [Identity]
        public Guid Id { get; set; }

        public string JobName { get; set; }

        public string Description { get; set; }

        public TimeSpan RunEvery { get; set; }
    }
}
