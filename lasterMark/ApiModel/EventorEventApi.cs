namespace lasterMark.ApiModel
{
    using System;

    using Newtonsoft.Json;

    public class EventorEventApi
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }
        
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}