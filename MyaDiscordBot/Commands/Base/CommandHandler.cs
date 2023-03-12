using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Blacklister;
using MyaDiscordBot.Models.SpamDetection;
using MyaDiscordBot.SelectEvent.Base;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace MyaDiscordBot.Commands.Base
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private IEnumerable<ICommand> commands;
        private IEnumerable<IButtonHandler> buttons;
        private IEnumerable<ISelectHandler> selects;
        private readonly HttpClient hc = new();
        private DateTime lastEntered = DateTime.MinValue;
        private int raidAlert = 0;
        private SocketGuildUser Admin;
        public CommandHandler(DiscordSocketClient client, IConfiguration configuration)
        {
            _client = client;
            hc.BaseAddress = new Uri("https://api.blacklister.xyz");
            _ = hc.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", configuration.BlackLister);
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
            foreach (ISelectHandler command in selects)
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
            _ = Process.Start("MyaDiscordBot.exe");
            Environment.Exit(0);
            return Task.CompletedTask;
        }

        private async Task _client_UserJoined(SocketGuildUser arg)
        {
            Console.WriteLine(arg.Id + " had joined the server.");
            await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", arg.Id + " had joined the server.\n", Encoding.UTF8);
            if (!arg.IsBot)
            {
                HttpResponseMessage response = await hc.GetAsync(arg.Id.ToString());
                CheckUserResponse result = JsonConvert.DeserializeObject<CheckUserResponse>(await response.Content.ReadAsStringAsync());
                if (result.Blacklisted)
                {
                    using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
                    Console.WriteLine(arg.Id + " is evil!! Kick him!!");
                    ISettingService setting = scope.Resolve<ISettingService>();
                    ServerSettings settings = setting.GetSettings(arg.Guild.Id);
                    if (settings.ChannelId != 0)
                    {
                        IChannel channel = await _client.GetChannelAsync(settings.ChannelId);
                        _ = await ((IMessageChannel)channel).SendMessageAsync("發現其他ser已經Blacklist的用戶：" + arg.Mention + "。");
                    }
                }
                if (await KickInvalidName(arg))
                {
                    using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
                    ISettingService setting = scope.Resolve<ISettingService>();
                    ServerSettings settings = setting.GetSettings(arg.Guild.Id);
                    if (settings.ChannelId != 0)
                    {
                        IChannel channel = await _client.GetChannelAsync(settings.ChannelId);
                        _ = await ((IMessageChannel)channel).SendMessageAsync("發現不合適名稱用戶：" + arg.Mention + "。");
                    }

                }
                if ((DateTime.Now - lastEntered).TotalMinutes < 1 && (DateTime.Now - arg.CreatedAt).TotalDays < 1)
                {
                    raidAlert++;
                }
                else
                {
                    raidAlert = 0;
                }
                lastEntered = DateTime.Now;
                if (raidAlert >= 5)
                {
                    using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
                    ISettingService setting = scope.Resolve<ISettingService>();
                    ServerSettings settings = setting.GetSettings(arg.Guild.Id);
                    if (Data.Instance.Youtube.Videos.Count >= 0)
                    {
                        if (Data.Instance.Youtube.Videos.Any(x => DateTime.Now.Date == x.ScheduledStartTime.ToLocalTime().Date && DateTime.Now >= x.ScheduledStartTime.ToLocalTime() && x.ScheduledStartTime.TimeOfDay <= new TimeSpan(23, 0, 0)))
                        {
                            //in Live, ignore server raid now
                            return;
                        }
                    }
                    if (settings.RaidKiller)
                    {
                        if (settings.ChannelId != 0)
                        {
                            IChannel channel = await _client.GetChannelAsync(settings.ChannelId);
                            SocketGuild guild = _client.GetGuild(arg.Guild.Id);
                            try
                            {
                                _ = await ((IMessageChannel)channel).SendMessageAsync("清理系統正在嘗試禁言" + arg.Mention);
                                await arg.SetTimeOutAsync(new TimeSpan(1, 0, 0));
                                _ = await arg.SendMessageAsync("你的行動疑似Server Raid, 已經被自動禁言！一小時後會自動解禁！");
                            }
                            catch (Exception)
                            {
                                _ = await ((IMessageChannel)channel).SendMessageAsync("清理系統無法踢出" + arg.Mention + "! 請確保有足夠權限！");
                            }
                        }
                    }
                }
            }
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message || string.IsNullOrEmpty(message.Content))
            {
                return;
            }
            try
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[" + message.Channel + "][" + arg.Author.Id + "]: " + message.Content + "\n", Encoding.UTF8);
                Console.WriteLine("[" + message.Channel.Name + "]" + message.Author.Id + ":" + message.Content);
                using (ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope())
                {
                    IAntiSpamService antiSpam = scope.Resolve<IAntiSpamService>();
                    if (antiSpam.IsSpam(message))
                    {
                        GuildEmote angry = _client.Guilds.FirstOrDefault(x => x.Id == 783913792668041216).Emotes.Where(x => x.Name.Contains("angry")).Last();
                        _ = await message.ReplyAsync("請唔好Spam！" + message.Author.Mention + angry.ToString());
                        await message.DeleteAsync();
                        return;
                    }
                    if (await antiSpam.IsScam(message))
                    {
                        GuildEmote angry = _client.Guilds.FirstOrDefault(x => x.Id == 783913792668041216).Emotes.Where(x => x.Name.Contains("angry")).Last();
                        _ = await message.ReplyAsync("請唔好發送病毒連接/詐騙鏈接！" + message.Author.Mention + angry.ToString());
                        await message.DeleteAsync();
                        return;
                    }
                    if(await antiSpam.IsPorn(message.Content))
                    {
                        GuildEmote angry = _client.Guilds.FirstOrDefault(x => x.Id == 783913792668041216).Emotes.Where(x => x.Name.Contains("angry")).Last();
                        _ = await message.ReplyAsync("請唔好發鹹網鏈接！" + message.Author.Mention + angry.ToString());
                        await message.DeleteAsync();
                        return;
                    }
                }
                if (message.MentionedUsers.Any(x => x.Id == _client.CurrentUser.Id))
                {
                    if (message.Content.Contains("食屎") || message.Content.Contains("fuck") || message.Content.Contains("白癡") || message.Content.Contains("是DD") || message.Content.Contains("去死"))
                    {
                        GuildEmote angry = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("angry")).Last();
                        GuildEmote fuck = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("fuck")).Last();
                        await message.AddReactionAsync(angry);
                        await message.AddReactionAsync(fuck);
                    }
                    else if (message.Content.Contains("屎") || message.Content.Contains("shit") || message.Content.Contains("米田共"))
                    {
                        GuildEmote sssmya = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("sssmya")).Last();
                        await message.AddReactionAsync(sssmya);
                    }
                    else if ((message.Content.Contains("愛") || message.Content.Contains("萬歲") || message.Content.Contains("love") || message.Content.Contains("鐘意")) && (message.Content.Contains("你") || message.Content.Contains("you") || message.Content.Contains("米亞") || message.Content.Contains("Mya")) && !message.Content.Contains("甘米"))
                    {
                        GuildEmote kiramya = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("kiramya")).Last();
                        await message.AddReactionAsync(kiramya);
                    }
                    else
                    {
                        GuildEmote what = _client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("what")).Last();
                        await message.AddReactionAsync(what);
                    }
                }
                else if (message.Content.StartsWith("$refreshCommand") && message.Author.Id == 294835963442757632)
                {
                    _ = await message.ReplyAsync("Job Executing... Please do not use any commands before job done!");
                    _ = Task.Run(async () =>
                    {
                        await UpdateCommands((message.Channel as SocketGuildChannel).Guild);
                        _ = await message.ReplyAsync("Job Done");
                    });
                }
                else if (message.Content.StartsWith("$ping"))
                {
                    _ = await message.ReplyAsync("Bot Status: " + _client.ConnectionState + "\nBot Delay: " + _client.Latency + "ms");
                }
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[" + message.Channel + "][Exception]: " + ex.ToString() + "\n", Encoding.UTF8);
            }
        }

        private Task _client_ButtonExecuted(SocketMessageComponent arg)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    foreach (IButtonHandler command in buttons)
                    {
                        if (arg.Data.Value != null)
                        {
                            if (command.CheckUsage(arg.Data.Value))
                            {
                                await command.Handle(arg, _client);
                            }
                        }
                        else if (arg.Data.CustomId != null)
                        {
                            if (command.CheckUsage(arg.Data.CustomId))
                            {
                                await command.Handle(arg, _client);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await arg.RespondAsync(ex.ToString());
                }
            });
            return Task.CompletedTask;
        }

        private Task _client_JoinedGuild(SocketGuild arg)
        {
            return UpdateCommands(arg);
        }

        private async Task UpdateCommands(SocketGuild arg = null)
        {
            await arg.DeleteApplicationCommandsAsync();
            foreach (ICommand command in commands)
            {
                try
                {
                    SlashCommandBuilder cmd = new()
                    {
                        Name = command.Name,
                        Description = command.Description
                    };
                    foreach (SlashCommandOptionBuilder o in command.Option)
                    {
                        _ = cmd.AddOptions(o);
                    }
                    _ = await arg.CreateApplicationCommandAsync(cmd.Build());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while creating command " + command.Name + "\n" + ex.Message);
                }
            }
        }

        private async Task client_Ready()
        {
            if (!Directory.Exists("save"))
            {
                _ = Directory.CreateDirectory("save");
            }
            commands = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(ICommand).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as ICommand);
            buttons = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(IButtonHandler).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as IButtonHandler);
            selects = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(ISelectHandler).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as ISelectHandler);
            await _client.SetGameAsync("頂米亞與甘米大冒險", type: ActivityType.Playing);
            IItemService i = Data.Instance.Container.Resolve<IItemService>();
            await i.SaveData();
        }

        private async Task client_SlashCommandExecuted(SocketSlashCommand arg)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + arg.CommandId + " triggered");
            if (arg.CommandName != "setting")
            {
                using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
                ISettingService setting = scope.Resolve<ISettingService>();
                if (!setting.CorrectChannel(arg, out ulong correctChannel))
                {
                    await arg.RespondAsync("請到" + MentionUtils.MentionChannel(correctChannel) + "使用Bot指令哦！", ephemeral: true);
                    return;
                }
            }
            ICommand command = commands.First(x => x.Name == arg.CommandName);
            try
            {
                await command.Handler(arg, _client);
            }
            catch (Exception ex)
            {
                await arg.RespondAsync(string.Join('\n', ex.ToString().Split("\n").Take(5)));
            }

        }

        private async Task<bool> KickInvalidName(IGuildUser arg)
        {
            if ((DateTime.Now - arg.CreatedAt).TotalDays < 7)
            {
                //scan for name
                //load blacklisted texts
                if (Data.Instance.BannedRegex.Count < 1)
                {
                    HttpResponseMessage response = await hc.GetAsync("https://raw.githubusercontent.com/mogade/badwords/master/en.txt");
                    foreach (string i in (await response.Content.ReadAsStringAsync()).Split("\n"))
                    {
                        Data.Instance.BannedRegex.Add(i);
                    }
                }
                foreach (string item in Data.Instance.BannedRegex)
                {
                    try
                    {
                        Regex r = new(item);
                        if (r.Match(arg.DisplayName).Success)
                        {
                            //auto kick when detected bad words in name and created less that 7 day
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //do nothing
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.ToString());
                    }

                }
                string[] customBlacklist = new string[] { "diueveryone" };
                foreach (string item in customBlacklist)
                {
                    if (arg.DisplayName == item)
                    {
                        //match bad word dictionary
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
