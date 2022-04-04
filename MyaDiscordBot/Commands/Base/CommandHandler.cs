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
            _client.JoinedGuild += _client_JoinedGuild;
            return Task.CompletedTask;
        }

        private async Task _client_JoinedGuild(SocketGuild arg)
        {
            foreach (var command in commands)
            {
                try
                {
                    var cmd = new SlashCommandBuilder();
                    cmd.Name = command.Name;
                    cmd.Description = command.Description;
                    foreach (var o in command.Option)
                    {
                        cmd.AddOptions(o);
                    }
                    await arg.CreateApplicationCommandAsync(cmd.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private async Task client_Ready()
        {
            var guild = _client.GetGuild(783913792668041216);
            var test = _client.GetGuild(904398494662529044);
            await _client.SetGameAsync("米亞RPG大冒險", type: ActivityType.Playing);
            commands = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(ICommand).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as ICommand);
            foreach (var command in commands)
            {
                try
                {
                    var cmd = new SlashCommandBuilder();
                    cmd.Name = command.Name;
                    cmd.Description = command.Description;
                    foreach (var o in command.Option)
                    {
                        cmd.AddOptions(o);
                    }
                    if(guild != null)
                    {
                        await guild.CreateApplicationCommandAsync(cmd.Build());
                    }
                    if(test != null)
                    {
                        await test.CreateApplicationCommandAsync(cmd.Build());
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private async Task client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            var command = commands.First(x => x.Name == arg.CommandName);
            try
            {
                await command.Handler(arg);
            }
            catch(Exception ex)
            {
                await arg.RespondAsync(ex.ToString());
            }

        }
    }
}
