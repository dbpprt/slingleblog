using System;
using MobileDB.Common.Attributes;

namespace SlingleBlog.Models
{
    public class PrecompiledPage
    {
        [Identity]
        public Guid Id { get; set; }

        public string Url { get; set; }

        public string Content { get; set; }

        public DateTime LastPrecompile { get; set; }

    }
}
