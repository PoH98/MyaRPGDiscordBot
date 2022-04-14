using Newtonsoft.Json;

namespace MyaDiscordBot.Models.Blacklister
{
    public class CheckUserResponse
    {
        [JsonProperty("blacklisted")]
        public bool Blacklisted { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("moderator")]
        public string Moderator { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("evidence")]
        public string Evidence { get; set; }
    }
}
