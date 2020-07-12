namespace lasterMark.Model
{
    using Newtonsoft.Json;

    public class CompetitorDto
    {
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "time_on_distance")]
        public string TimeOfDistance { get; set; }

        [JsonProperty(PropertyName = "distance_name")]
        public string Distance { get; set; }
    }
}