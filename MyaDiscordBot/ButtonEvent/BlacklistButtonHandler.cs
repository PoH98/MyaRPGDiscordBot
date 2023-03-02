using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Blacklister;
using Newtonsoft.Json;
using System.Text;

namespace MyaDiscordBot.ButtonEvent
{
    public class BlacklistButtonHandler : IButtonHandler
    {
        private readonly IConfiguration configuration;
        public BlacklistButtonHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("ban-");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            string[] parts = message.Data.CustomId.Split('-');
            string reason = parts[2] switch
            {
                "1" => "[AUTO MOD] Posting Ads in Guild",
                "2" => "[AUTO MOD] Posting Scam links in Guild",
                "3" => "[AUTO MOD] Testing from Discord.NET HttpClient",
                _ => "[AUTO MOD] Fast Spamming",
            };
            if (message.User.Id == 294835963442757632)
            {
                try
                {
                    ulong uid = Convert.ToUInt64(parts[1]);
                    SocketGuildUser u = (message.Channel as SocketGuildChannel).Guild.GetUser(uid);
                    //Author, just timeout
                    List<ulong> ids = new();
                    if (u.Roles != null)
                    {
                        foreach (SocketRole r in u.Roles)
                        {
                            if (!r.IsEveryone)
                            {
                                ids.Add(r.Id);
                            }
                        }
                    }
                    await u.RemoveRolesAsync(ids);
                    await u.SetTimeOutAsync(new TimeSpan(24, 0, 0));
                    await message.RespondAsync("User timeout set", ephemeral: true);
                    return;
                }
                catch (Exception ex)
                {
                    await message.RespondAsync(ex.ToString(), ephemeral: true);
                }

            }
            HttpClient hc = new();
            _ = hc.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", configuration.BlackLister);
            string inputJson = JsonConvert.SerializeObject(new ReportUser
            {
                Reason = reason
            });
            StringContent content = new(inputJson, Encoding.UTF8, "application/json");
            await message.DeferLoadingAsync();
            HttpResponseMessage response = await hc.PostAsync("https://api.blacklister.xyz/report/" + parts[1], content);
            string result = await response.Content.ReadAsStringAsync();
            bool blackListed = false;
            if (result.Contains("\"err\":false"))
            {
                //success
                blackListed = true;
            }
            ulong userId = Convert.ToUInt64(parts[1]);
            SocketGuildUser user = (message.Channel as SocketGuildChannel).Guild.GetUser(userId);
            Console.WriteLine("Blacklisted Status: " + blackListed);
            if (blackListed)
            {
                try
                {
                    await user.KickAsync();
                    _ = await message.ModifyOriginalResponseAsync(x => x.Content = "已經黑名單" + user.DisplayName + "！自動踢出用戶！");
                    Console.WriteLine("Kicked");
                }
                catch (Exception ex)
                {
                    _ = await message.ModifyOriginalResponseAsync(x => x.Content = "已經黑名單" + user.DisplayName + "！用戶踢出失敗！錯誤原因：" + ex.Message);
                    Console.WriteLine("Not Kicked");
                }
            }
            else
            {
                try
                {
                    await user.KickAsync();
                    _ = await message.ModifyOriginalResponseAsync(x => x.Content = "黑名單" + user.DisplayName + "失敗！自動踢出用戶！");
                }
                catch (Exception ex)
                {
                    _ = await message.ModifyOriginalResponseAsync(x => x.Content = "黑名單" + user.DisplayName + "失敗！用戶踢出失敗！錯誤原因：" + ex.Message);
                }
            }
        }
    }
}
