using System.Collections.Generic;

namespace lasterMark.ApiModel
{
    using Newtonsoft.Json;

    class CompetitorListApi
    {
        [JsonProperty(PropertyName = "ok")]
        public bool Ok { get; set; }

        [JsonProperty(PropertyName = "competitors")]
        public List<CompetitorApi> Competitors { get; set; }
    }
}