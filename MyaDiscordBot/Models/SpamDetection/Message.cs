using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models.SpamDetection
{
    public class Message
    {
        /// <summary>
        /// Sent for same times
        /// </summary>
        public int SameTimes { get; set; }
        public string Content { get; set; }
        public ulong Id { get; set; }
    }
}
