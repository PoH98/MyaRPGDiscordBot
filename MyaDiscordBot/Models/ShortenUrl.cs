using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    internal class ShortenUrl
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("list")]
        public List<string> List { get; set; }

        [JsonProperty("matching_attributes")]
        public List<string> MatchingAttributes { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
