namespace MyaDiscordBot.Models
{
    public interface IConfiguration
    {
        string Token { get; set; }
        string BlackLister { get; set; }
        ulong AdminId { get; set; }
        Dictionary<string, int> LV { get; set; }
    }

    public class Configuration : IConfiguration
    {
        public string Token { get; set; }
        public Dictionary<string, int> LV { get; set; }
        public ulong AdminId { get; set; }
        public string BlackLister { get; set; }
    }
}
