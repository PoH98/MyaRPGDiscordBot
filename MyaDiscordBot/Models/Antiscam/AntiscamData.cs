using Newtonsoft.Json;

namespace MyaDiscordBot.Models.Antiscam
{
    internal class AntiscamData
    {
        [JsonProperty("domains")]
        public List<string> Domains { get; set; }
    }
}
