﻿namespace MyaDiscordBot.Models
{
    public class MarketData
    {
        public Guid Id { get; set; }
        public string PlayerId { get; set; }
        public ulong DiscordSellerId { get; set; }
        public string ResourceId { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
    }
}
