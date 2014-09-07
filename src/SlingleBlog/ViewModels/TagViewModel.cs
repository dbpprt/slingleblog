using Newtonsoft.Json;

namespace SlingleBlog.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TagViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("posts")]
        public int Posts { get; set; }
    }
}
