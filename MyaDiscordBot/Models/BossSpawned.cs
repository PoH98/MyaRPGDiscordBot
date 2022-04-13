using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class BossSpawned
    {
        public Guid Id { get; set; }
        public Enemy Enemy { get; set; }
        public DateTime ExpiredTime { get; set; } = DateTime.Now.AddDays(1);
        public ulong GuildId { get; set; }
    }
}
