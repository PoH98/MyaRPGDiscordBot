using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent;
using MyaDiscordBot.GameLogic.Services;
using System.Text.RegularExpressions;

namespace MyaDiscordBot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private IEnumerable<ICommand> commands;
        private IEnumerable<IButtonHandler> buttons;
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
            _client.ButtonExecuted += _client_ButtonExecuted;
            _client.MessageReceived += _client_MessageReceived;
            return Task.CompletedTask;
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;
            if (message.MentionedUsers.Any(x => x.Id == _client.CurrentUser.Id))
            {
                if (message.Content.Contains("食屎") || message.Content.Contains("fuck") || message.Content.Contains("白癡") || message.Content.Contains("是DD") || message.Content.Contains("去死"))
                {
                    var angry = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("veryangry")).Last();
                    var fuck = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("fuck")).Last();
                    await message.AddReactionAsync(angry);
                    await message.AddReactionAsync(fuck);
                }
                else if (message.Content.Contains("屎"))
                {
                    var sssmya = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("sssmya")).Last();
                    await message.AddReactionAsync(sssmya);
                }
                else if ((message.Content.Contains("愛") || message.Content.Contains("love") || message.Content.Contains("鐘意")) && (message.Content.Contains("你") || message.Content.Contains("you") || message.Content.Contains("米亞") || message.Content.Contains("Mya")) && (!message.Content.Contains("甘米")))
                {
                    var kiramya = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("kiramya")).Last();
                    await message.AddReactionAsync(kiramya);
                }
                else
                {
                    var what = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("what")).Last();
                    await message.AddReactionAsync(what);
                }
            }
        }

        private async Task _client_ButtonExecuted(SocketMessageComponent arg)
        {
            foreach (var command in buttons)
            {
                if (arg.Data.Value != null)
                {
                    if (command.CheckUsage(arg.Data.Value))
                    {
                        await command.Handle(arg, _client);
                        //already handled, no need do any more thing
                        return;
                    }
                }
                else if (arg.Data.CustomId != null)
                {
                    if (command.CheckUsage(arg.Data.CustomId))
                    {
                        await command.Handle(arg, _client);
                        //already handled, no need do any more thing
                        return;
                    }
                }
            }
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
            if (!Directory.Exists("save"))
            {
                Directory.CreateDirectory("save");
            }

            commands = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(ICommand).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as ICommand);
            buttons = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(IButtonHandler).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as IButtonHandler);
            await _client.SetGameAsync("頂米亞與甘米大冒險", type: ActivityType.Playing);
            var i = Data.Instance.Container.Resolve<IItemService>();
            await i.SaveData();
            foreach (var db in Directory.GetFiles("save", "*.json"))
            {
                var guild = _client.GetGuild(ulong.Parse(Regex.Match(db, @"\d+").Value));
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
                        _ = guild.CreateApplicationCommandAsync(cmd.Build());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private async Task client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            if(arg.CommandName != "setting")
            {
                using (var scope = Data.Instance.Container.BeginLifetimeScope())
                {
                    var setting = scope.Resolve<ISettingService>();
                    if (!setting.CorrectChannel(arg, out ulong correctChannel))
                    {
                        await arg.RespondAsync("請到" + MentionUtils.MentionChannel(correctChannel) + "使用Bot指令哦！", ephemeral: true);
                        return;
                    }
                }
            }
            var command = commands.First(x => x.Name == arg.CommandName);
            try
            {
                await command.Handler(arg, _client);
            }
            catch (Exception ex)
            {
                await arg.RespondAsync(string.Join('\n', ex.ToString().Split("\n").Take(5)));
            }

        }
    }
}
