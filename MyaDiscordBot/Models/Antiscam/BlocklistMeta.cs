using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models.Antiscam
{
    public class BlocklistMeta
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("raw_url")]
        public string RawUrl;

        [JsonProperty("updated")]
        public DateTime Updated;

        [JsonProperty("updated_format")]
        public string UpdatedFormat;

        [JsonProperty("lines")]
        public int Lines;

        [JsonProperty("lines_format")]
        public string LinesFormat;

        [JsonProperty("size")]
        public int Size;

        [JsonProperty("size_format")]
        public string SizeFormat;
    }
}
