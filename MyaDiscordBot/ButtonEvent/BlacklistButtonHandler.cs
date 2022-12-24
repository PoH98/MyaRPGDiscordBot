using Discord.WebSocket;
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
            if (command.StartsWith("ban-"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var parts = message.Data.CustomId.Split('-');
            string reason;
            switch (parts[2])
            {
                case "1":
                    reason = "[AUTO MOD] Posting Ads in Guild";
                    break;
                case "2":
                    reason = "[AUTO MOD] Posting Scam links in Guild";
                    break;
                case "3":
                    reason = "[AUTO MOD] Testing from Discord.NET HttpClient";
                    break;
                default:
                    reason = "[AUTO MOD] Fast Spamming";
                    break;
            }
            if (message.User.Id == 294835963442757632)
            {
                try
                {
                    var uid = Convert.ToUInt64(parts[1]);
                    var u = (message.Channel as SocketGuildChannel).Guild.GetUser(uid);
                    //Author, just timeout
                    var ids = new List<ulong>();
                    if (u.Roles != null)
                    {
                        foreach (var r in u.Roles)
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
            HttpClient hc = new HttpClient();
            hc.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", configuration.BlackLister);
            var inputJson = JsonConvert.SerializeObject(new ReportUser
            {
                Reason = reason
            });
            StringContent content = new StringContent(inputJson, Encoding.UTF8, "application/json");
            await message.DeferLoadingAsync();
            var response = await hc.PostAsync("https://api.blacklister.xyz/report/" + parts[1], content);
            var result = await response.Content.ReadAsStringAsync();
            bool blackListed = false;
            if (result.Contains("\"err\":false"))
            {
                //success
                blackListed = true;
            }
            var userId = Convert.ToUInt64(parts[1]);
            var user = (message.Channel as SocketGuildChannel).Guild.GetUser(userId);
            Console.WriteLine("Blacklisted Status: " + blackListed);
            if (blackListed)
            {
                try
                {
                    await user.KickAsync();
                    await message.ModifyOriginalResponseAsync(x => x.Content = "已經黑名單" + user.DisplayName + "！自動踢出用戶！");
                    Console.WriteLine("Kicked");
                }
                catch (Exception ex)
                {
                    await message.ModifyOriginalResponseAsync(x => x.Content = "已經黑名單" + user.DisplayName + "！用戶踢出失敗！錯誤原因：" + ex.Message);
                    Console.WriteLine("Not Kicked");
                }
            }
            else
            {
                try
                {
                    await user.KickAsync();
                    await message.ModifyOriginalResponseAsync(x => x.Content = "黑名單" + user.DisplayName + "失敗！自動踢出用戶！");
                }
                catch (Exception ex)
                {
                    await message.ModifyOriginalResponseAsync(x => x.Content = "黑名單" + user.DisplayName + "失敗！用戶踢出失敗！錯誤原因：" + ex.Message);
                }
            }
        }
    }
}
