namespace MyaDiscordBot.Models.SpamDetection
{
    public class Message
    {
        /// <summary>
        /// Sent for same times
        /// </summary>
        public int SameTimes { get; set; }
        public string Content { get; set; }
        public Guid Id { get; set; }
        public ulong UserId { get; set; }
        public ulong AverageTimeBetwen { get; set; }
        public long LastMessageTime { get; set; }
    }
}
