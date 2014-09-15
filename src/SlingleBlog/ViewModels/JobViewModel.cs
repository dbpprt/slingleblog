using System;
using Newtonsoft.Json;

namespace SlingleBlog.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JobViewModel
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }
        
        [JsonProperty("jobName")]
        public string JobName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("interval")]
        public TimeSpan Interval { get; set; }

        [JsonProperty("nextScheduledExecution")]
        public DateTime NextScheduledExecution { get; set; }
    }
}
