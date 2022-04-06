using Discord.WebSocket;

namespace MyaDiscordBot.ButtonEvent
{
    public interface IButtonHandler
    {
        bool CheckUsage(string command);
        Task Handle(SocketMessageComponent message, DiscordSocketClient client);
    }
}
