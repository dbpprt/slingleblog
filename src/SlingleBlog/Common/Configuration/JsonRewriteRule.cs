using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlingleBlog.Common.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RewriteRule : IRewriteRule
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("constraint")]
        public string Constraint { get; set; }

        [JsonProperty("template")]
        public string Template { get; private set; }

        [JsonProperty("rewrite-to")]
        public string RewriteTo { get; set; }
    }
}
