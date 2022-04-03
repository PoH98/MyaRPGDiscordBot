using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class Save
    {
        public int CurrentStage { get; set; }
        public Enemy CurrentBoss { get; set; }
        public int EnemyLeft { get; set; }
    }
}
