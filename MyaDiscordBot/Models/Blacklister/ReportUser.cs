using Newtonsoft.Json;

namespace MyaDiscordBot.Models.Blacklister
{
    public class ReportUser
    {
        [JsonProperty("reason")]
        public string Reason { get; set; } = "";

        [JsonProperty("evidence")]
        public string Evidence { get; set; } = "https://evidence.blacklister.xyz/no-evidence.png";

        [JsonProperty("anythingelse")]
        public string AnythingElse { get; set; } = "";
        [JsonProperty("reporter")]
        public object Reporter { get; set; } = new { };
    }
}
