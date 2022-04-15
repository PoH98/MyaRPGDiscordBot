using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models.Blacklister
{
    public class ReportUser
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("evidence")]
        public string Evidence { get; set; }

        [JsonProperty("anythingelse")]
        public string Anythingelse { get; set; }
    }
}
