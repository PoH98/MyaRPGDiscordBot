using Autofac;
using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private IEnumerable<ICommand> commands;
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.SlashCommandExecuted += client_SlashCommandExecuted;
            _client.Ready += client_Ready;
            return Task.CompletedTask;
        }

        private async Task client_Ready()
        {
            var guild = _client.GetGuild(783913792668041216);
            commands = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(ICommand).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as ICommand);
            foreach (var command in commands)
            {
                var cmd = new SlashCommandBuilder();
                cmd.Name = command.Name;
                cmd.Description = command.Description;
                foreach(var o in command.Option)
                {
                    cmd.AddOptions(o);
                }
                await guild.CreateApplicationCommandAsync(cmd.Build());
            }
        }

        private Task client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            var command = commands.First(x => x.Name == arg.CommandName);
            return command.Handler(arg);
        }
    }
}
