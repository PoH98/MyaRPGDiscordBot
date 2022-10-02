using Newtonsoft.Json;

namespace MyaDiscordBot.Models.MyaWebsite
{
    public class YTData
    {
        [JsonProperty("TimeTableUrl")]
        public string TimeTableUrl { get; set; }

        [JsonProperty("Videos")]
        public List<Video> Videos { get; set; }
    }

    public class Video
    {
        [JsonProperty("Thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("ScheduledStartTime")]
        public DateTime ScheduledStartTime { get; set; }
    }
}
