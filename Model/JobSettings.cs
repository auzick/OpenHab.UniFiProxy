namespace OpenHab.UniFiProxy.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public partial class JobSettings
    {
        [JsonPropertyName("jobs")]
        public List<Job> Jobs { get; set; }

        [JsonPropertyName("pollInterval")]
        public int PollInterval { get; set; }


        public JobSettings()
        {
            Jobs = new List<Job>();
        }

        public partial class Job
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("item")]
            public string Item { get; set; }

            [JsonPropertyName("frequency")]
            public long Frequency { get; set; }

            [JsonIgnore]
            public DateTime LastRun { get; set; }

            [JsonIgnore]
            public string LastValue { get; set; }

            public Job()
            {
                LastRun = DateTime.Now;
                LastValue = String.Empty;
            }
        }

    }
}
