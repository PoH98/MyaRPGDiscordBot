using Newtonsoft.Json;

namespace MyaDiscordBot.Models.Antiscam
{
    public class PornMetaJson
    {
        [JsonProperty("blocklist")]
        public BlocklistMeta Blocklist;
    }
}
