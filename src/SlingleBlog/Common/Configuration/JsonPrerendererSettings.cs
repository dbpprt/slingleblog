using Newtonsoft.Json;

namespace SlingleBlog.Common.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonPrerendererSettings : IPrerendererSettings
    {
        [JsonProperty("recompile-page-after-hours")]
        public int RecompileAfterHours { get; set; }

        [JsonProperty("dom-variable")]
        public string DomVariable { get; set; }

        [JsonProperty("load-timeout")]
        public int Timeout { get; set; }
    }
}
