using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Commands
{
    public class AddJoke : ICommand
    {
        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1] { new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.String).WithName("Joke Content").WithRequired(true) };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            if (!File.Exists("joke.txt"))
            {
                File.WriteAllText("joke.txt","");
            }
            await File.AppendAllTextAsync("joke.txt", ((string)command.Data.Options.First().Value) + "\n");
            await command.RespondAsync("米亞非常高興你卑佢知道有咁個好笑野發生！", ephemeral: true);
        }
    }
}
