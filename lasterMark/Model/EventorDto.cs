namespace lasterMark.Model
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class EventorDto
    {
        [JsonProperty(PropertyName = "ok")]
        public bool Ok { get; set; }

        [JsonProperty(PropertyName = "events")]
        public List<EventorEventDto> Events { get; set; }
    }
}
