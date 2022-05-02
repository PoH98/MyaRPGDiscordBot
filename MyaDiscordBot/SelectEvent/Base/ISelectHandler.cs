using Discord.WebSocket;

namespace MyaDiscordBot.SelectEvent
{
    public interface ISelectHandler
    {
        bool CheckUsage(string command);
        Task Handle(SocketMessageComponent message, DiscordSocketClient client);
    }
}
