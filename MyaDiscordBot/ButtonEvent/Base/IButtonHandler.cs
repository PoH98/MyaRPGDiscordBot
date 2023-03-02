using Discord.WebSocket;

namespace MyaDiscordBot.ButtonEvent.Base
{
    public interface IButtonHandler
    {
        bool CheckUsage(string command);
        Task Handle(SocketMessageComponent message, DiscordSocketClient client);
    }
}
