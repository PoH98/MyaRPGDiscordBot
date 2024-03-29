﻿namespace MyaDiscordBot.Models
{
    public class BattleResult
    {
        internal bool IsVictory { get; set; }
        internal int DamageDealt { get; set; }
        internal int DamageReceived { get; set; }
        internal Dictionary<ItemEquip, int> ItemsUsed { get; set; } = new Dictionary<ItemEquip, int>();
    }
}
