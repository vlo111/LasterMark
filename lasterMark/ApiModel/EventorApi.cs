namespace lasterMark.ApiModel
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class EventorApi
    {
        [JsonProperty(PropertyName = "ok")]
        public bool Ok { get; set; }

        [JsonProperty(PropertyName = "events")]
        public List<EventorEventApi> Events { get; set; }
    }
}
