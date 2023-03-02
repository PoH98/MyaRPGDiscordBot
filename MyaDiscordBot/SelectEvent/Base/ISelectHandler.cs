using Discord.WebSocket;

namespace MyaDiscordBot.SelectEvent.Base
{
    public interface ISelectHandler
    {
        bool CheckUsage(string command);
        Task Handle(SocketMessageComponent message, DiscordSocketClient client);
    }
}
