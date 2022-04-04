using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class BattleResult
    {
        internal bool IsVictory { get; set; }
        internal int DamageDealt { get; set; }
        internal int DamageReceived { get; set; }
        internal Dictionary<ItemEquip, int> ItemsUsed { get; set; } = new Dictionary<ItemEquip, int>();
    }
}
