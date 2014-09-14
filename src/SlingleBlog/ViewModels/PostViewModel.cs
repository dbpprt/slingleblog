using System;
using Newtonsoft.Json;

namespace SlingleBlog.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PostViewModel
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("pubdate")]
        public DateTime PubDate { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
