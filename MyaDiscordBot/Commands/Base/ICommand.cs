using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands
{
    internal interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Task Handler(SocketSlashCommand command);
        IEnumerable<SlashCommandOptionBuilder> Option { get; }
    }
}
