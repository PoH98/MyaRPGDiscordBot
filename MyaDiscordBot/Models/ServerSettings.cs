namespace MyaDiscordBot.Models
{
    public class ServerSettings
    {
        public Guid Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public bool RaidKiller { get; set; }
    }
}
