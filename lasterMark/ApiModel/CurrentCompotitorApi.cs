namespace lasterMark.ApiModel
{
    using Newtonsoft.Json;

    public class CurrentCompotitorApi
    {
        [JsonProperty(PropertyName = "ok")]
        public bool Ok { get; set; }

        [JsonProperty(PropertyName = "competitor")]
        public CompetitorApi Competitor { get; set; }
    }
}
