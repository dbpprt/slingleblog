using Newtonsoft.Json;

namespace SlingleBlog.Common.Configuration
{
    public class JsonAccount : IAccount
    {
        [JsonProperty("displayname")]
        public string DisplayName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }
        
        [JsonProperty("roles")]
        public string[] Roles { get; set; }
    }
}
