using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Blacklister;
using MyaDiscordBot.Models.SpamDetection;
using MyaDiscordBot.SelectEvent;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MyaDiscordBot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private IEnumerable<ICommand> commands;
        private IEnumerable<IButtonHandler> buttons;
        private IEnumerable<ISelectHandler> selects;
        private HttpClient hc = new HttpClient();
        public CommandHandler(DiscordSocketClient client, IConfiguration configuration)
        {
            _client = client;
            hc.BaseAddress = new Uri("https://api.blacklister.xyz");
            hc.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", configuration.BlackLister);
        }

        public void InstallCommands()
        {
            // Hook the MessageReceived event into our command handler
            _client.SlashCommandExecuted += client_SlashCommandExecuted;
            _client.Ready += client_Ready;
            _client.JoinedGuild += _client_JoinedGuild;
            _client.UserJoined += _client_UserJoined;
            _client.ButtonExecuted += _client_ButtonExecuted;
            _client.MessageReceived += _client_MessageReceived;
            _client.Disconnected += _client_Disconnected;
            _client.SelectMenuExecuted += _client_SelectMenuExecuted;
        }

        private Task _client_SelectMenuExecuted(SocketMessageComponent arg)
        {
            foreach (var command in selects)
            {
                if (arg.Data.Values != null)
                {
                    if (command.CheckUsage(arg.Data.Values.First()))
                    {
                        _ = command.Handle(arg, _client);
                        //already handled, no need do any more thing
                        return Task.CompletedTask;
                    }
                }
                else if (arg.Data.CustomId != null)
                {
                    if (command.CheckUsage(arg.Data.CustomId))
                    {
                        _ = command.Handle(arg, _client);
                        //already handled, no need do any more thing
                        return Task.CompletedTask;
                    }
                }
            }
            return Task.CompletedTask;
        }

        private Task _client_Disconnected(Exception arg)
        {
            Process.Start("MyaDiscordBot.exe");
            Environment.Exit(0);
            return Task.CompletedTask;
        }

        private async Task _client_UserJoined(SocketGuildUser arg)
        {
            Console.WriteLine(arg.Id + " had joined the server.");
            if (!arg.IsBot)
            {
                var response = await hc.GetAsync(arg.Id.ToString());
                var result = JsonConvert.DeserializeObject<CheckUserResponse>(await response.Content.ReadAsStringAsync());
                if (result.Blacklisted)
                {
                    Console.WriteLine(arg.Id + " is evil!! Kick him!!");
                    await arg.KickAsync(result.Reason + " in other server on " + result.Date);
                }
            }
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;
            using (var scope = Data.Instance.Container.BeginLifetimeScope())
            {
                var antiSpam = scope.Resolve<IAntiSpamService>();
                if (antiSpam.IsSpam(message))
                {
                    await message.DeleteAsync();
                    return;
                }
            }
            if (message.MentionedUsers.Any(x => x.Id == _client.CurrentUser.Id))
            {
                if (message.Content.Contains("食屎") || message.Content.Contains("fuck") || message.Content.Contains("白癡") || message.Content.Contains("是DD") || message.Content.Contains("去死"))
                {
                    var angry = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("angry")).Last();
                    var fuck = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("fuck")).Last();
                    await message.AddReactionAsync(angry);
                    await message.AddReactionAsync(fuck);
                }
                else if (message.Content.Contains("屎") || message.Content.Contains("shit") || message.Content.Contains("米田共"))
                {
                    var sssmya = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("sssmya")).Last();
                    await message.AddReactionAsync(sssmya);
                }
                else if ((message.Content.Contains("愛") || message.Content.Contains("萬歲") || message.Content.Contains("love") || message.Content.Contains("鐘意")) && (message.Content.Contains("你") || message.Content.Contains("you") || message.Content.Contains("米亞") || message.Content.Contains("Mya")) && (!message.Content.Contains("甘米")))
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
            else if (message.Content.StartsWith("$refreshCommand") && message.Author.Id == 294835963442757632)
            {
                await message.ReplyAsync("Job Executing... Please do not use any commands before job done!");
                _ = Task.Run(async () =>
                {
                    await UpdateCommands((message.Channel as SocketGuildChannel).Guild);
                    await message.ReplyAsync("Job Done");
                });
            }
            else if (message.Content.StartsWith("$ping"))
            {
                await message.ReplyAsync("Bot Status: " + _client.ConnectionState + "\nBot Delay: " + _client.Latency + "ms");
            }
        }

        private Task _client_ButtonExecuted(SocketMessageComponent arg)
        {
            foreach (var command in buttons)
            {
                if (arg.Data.Value != null)
                {
                    if (command.CheckUsage(arg.Data.Value))
                    {
                        _ = command.Handle(arg, _client);
                        //already handled, no need do any more thing
                        return Task.CompletedTask;
                    }
                }
                else if (arg.Data.CustomId != null)
                {
                    if (command.CheckUsage(arg.Data.CustomId))
                    {
                        _ = command.Handle(arg, _client);
                        //already handled, no need do any more thing
                        return Task.CompletedTask;
                    }
                }
            }
            return Task.CompletedTask;
        }

        private Task _client_JoinedGuild(SocketGuild arg)
        {
            return UpdateCommands(arg);
        }

        private async Task UpdateCommands(SocketGuild arg = null)
        {
            await arg.DeleteApplicationCommandsAsync();
            foreach(var command in commands)
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
                    Console.WriteLine(ex.ToString());
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
            selects = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(ISelectHandler).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as ISelectHandler);
            await _client.SetGameAsync("頂米亞與甘米大冒險", type: ActivityType.Playing);
            var i = Data.Instance.Container.Resolve<IItemService>();
            await i.SaveData();
        }

        private async Task client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + arg.CommandId + " triggered");
            if (arg.CommandName != "setting")
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
