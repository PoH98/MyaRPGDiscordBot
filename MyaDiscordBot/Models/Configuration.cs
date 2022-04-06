namespace MyaDiscordBot.Models
{
    public interface IConfiguration
    {
        string Token { get; set; }
    }

    public class Configuration : IConfiguration
    {
        public string Token { get; set; }
    }
}
