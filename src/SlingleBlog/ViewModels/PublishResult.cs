using Newtonsoft.Json;

namespace SlingleBlog.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PublishResult
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
