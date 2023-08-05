using Discord.WebSocket;
using LiteDB;
using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Antiscam;
using MyaDiscordBot.Models.SpamDetection;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IAntiSpamService
    {
        bool IsSpam(SocketUserMessage message);
        Task<bool> IsScam(SocketUserMessage message);
        Task<bool> IsRude(string message);
        Task<bool> IsPorn(string message);
    }
    public class AntiSpamService : IAntiSpamService
    {
        public async Task<bool> IsPorn(string message)
        {
            try
            {
                await FetchAPIData();
                var urls = Data.Instance.PornList.Where(x => message.Contains(x));
                if (urls.Count() > 0)
                {
                    foreach (var url in urls)
                    {
                        var result = Regex.Match(message, $"(https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")}|https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")})");
                        if (result.Success)
                        {
                            return true;
                        };
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Exception]: " + ex.ToString() + "\n", Encoding.UTF8);
                return await IsPorn(message);
            }
        }

        public async Task<bool> IsRude(string message)
        {
            Match match = Regex.Match(message, @"(https:\/\/)?(www\.)?(((discord(app)?)?\.com\/invite)|((discord(app)?)?\.gg))\/(?<invite>.+)");
            if (match.Success)
            {
                if (match.Groups.TryGetValue("invite", out Group inviteurl))
                {
                    HttpClient client = new();
                    HttpResponseMessage result = await client.GetAsync("https://discord.com/api/v9/invites/" + inviteurl.Value);
                    DiscordInvite di = JsonConvert.DeserializeObject<DiscordInvite>(await result.Content.ReadAsStringAsync());
                    result = await client.GetAsync("https://api.phish.gg/server?id=" + di.Guild.Id);
                    PhishGG pg = JsonConvert.DeserializeObject<PhishGG>(await result.Content.ReadAsStringAsync());
                    return pg.Match;
                }
            }
             await FetchAPIData();
            if (Data.Instance.ScamList.Any(x => x.Domains.Any(y => message.Contains(y))))
            {
                if (message.Contains("https://cdn.discordapp.com/") || message.Contains("https://discord.com/"))
                {
                    //cdn, not scam lol
                    return false;
                }
                return true;
            }
            var urls = Data.Instance.PornList.Where(x => message.Contains(x));
            if (urls.Count() > 0)
            {
                foreach (var url in urls)
                {
                    var result = Regex.Match(message, $"(https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")}|https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")})");
                    if (result.Success)
                    {
                        return true;
                    };
                }
            }
            match = Regex.Match(message, @"(https?:\/\/)?(www[.])?(telegram|t)\.me\/([a-zA-Z0-9_-]*)\/?$");
            if(match.Success)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsScam(SocketUserMessage message)
        {
            try
            {
                Match match = Regex.Match(message.Content, @"(https:\/\/)?(www\.)?(((discord(app)?)?\.com\/invite)|((discord(app)?)?\.gg))\/(?<invite>.+)");
                if (match.Success)
                {
                    if (match.Groups.TryGetValue("invite", out Group inviteurl))
                    {
                        HttpClient client = new();
                        HttpResponseMessage result = await client.GetAsync("https://discord.com/api/v9/invites/" + inviteurl.Value);
                        DiscordInvite di = JsonConvert.DeserializeObject<DiscordInvite>(await result.Content.ReadAsStringAsync());
                        result = await client.GetAsync("https://api.phish.gg/server?id=" + di.Guild.Id);
                        PhishGG pg = JsonConvert.DeserializeObject<PhishGG>(await result.Content.ReadAsStringAsync());
                        return pg.Match;
                    }
                }
                await FetchAPIData();
                if (Data.Instance.ScamList.Any(x => x.Domains.Any(y => message.Content.Contains(y + '/'))))
                {
                    if (message.Content.Contains("https://cdn.discordapp.com/") || message.Content.Contains("https://discord.com/"))
                    {
                        //cdn, not scam lol
                        return false;
                    }
                    return true;
                }
                match = Regex.Match(message.Content, @"(https?:\/\/)?(www[.])?(telegram|t)\.me\/([a-zA-Z0-9_-]*)\/?$");
                return match.Success;
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[" + message.Channel + "][Exception]: " + ex.ToString() + "\n", Encoding.UTF8);
                return await IsScam(message);
            }

        }


        public bool IsSpam(SocketUserMessage message)
        {
            using (LiteDatabase db = new("Filename=save\\" + (message.Channel as SocketGuildChannel).Guild.Id + ".db;connection=shared"))
            {
                ILiteCollection<Message> col = db.GetCollection<Message>("spam");
                Message data = col.FindOne(x => x.UserId == message.Author.Id);
                bool mon = false;
                if (data == null)
                {
                    _ = col.Insert(new Message() { UserId = message.Author.Id, SameTimes = 0, Content = message.Content, LastMessageTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() });
                }
                else
                {
                    if ((((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - data.LastMessageTime) <= 2)
                    {
                        data.SameTimes++;
                        mon = true;
                    }
                    if (data.Content == message.Content)
                    {
                        data.SameTimes++;
                        mon = true;
                    }
                    if (!mon)
                    {
                        data.SameTimes = 0;
                    }
                    _ = col.Update(new Message() { UserId = message.Author.Id, SameTimes = data.SameTimes, Content = message.Content, LastMessageTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() });
                    if (data.SameTimes >= 3)
                    {
                        return true;
                    }
                }
                string[] splits = message.Content.Split("\n");
                if (splits.Count() > 10)
                {
                    if (splits.GroupBy(s => s.ToLower()).Count() <= splits.Count() / 3)
                    {
                        return true;
                    }
                }
                return false;
            }
               

        }

        private async Task FetchAPIData()
        {
            if (Data.Instance.ScamList.Count < 1)
            {
                //fetch scamlist
                HttpClient client = new();
                HttpResponseMessage result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/domain-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
                result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/suspicious-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
            }

            if (Data.Instance.PornList.Count < 1)
            {
                //fetch pornlist
                HttpClient client = new();
                HttpResponseMessage result = await client.GetAsync("https://raw.githubusercontent.com/Bon-Appetit/porn-domains/master/block.txt");
                var list = await result.Content.ReadAsStringAsync();
                foreach (var item in list.Split("\n"))
                {
                    if (item.Trim().Length > 0 && !item.Contains("pixiv"))
                    {
                        Data.Instance.PornList.Add(item.Trim());
                    }
                }
            }
        }
    }
}
