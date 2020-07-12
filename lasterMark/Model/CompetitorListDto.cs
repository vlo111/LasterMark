using System.Collections.Generic;

namespace lasterMark.Model
{
    using Newtonsoft.Json;

    class CompetitorListDto
    {
        [JsonProperty(PropertyName = "ok")]
        public bool Ok { get; set; }

        [JsonProperty(PropertyName = "competitors")]
        public List<CompetitorDto> Competitors { get; set; }
    }
}