using Autofac;
using Discord;
using Discord.WebSocket;
using LiteDB;
using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Antiscam;
using MyaDiscordBot.Models.SpamDetection;
using Newtonsoft.Json;
using Quartz;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IAntiSpamService
    {
        bool IsTextSpam(SocketUserMessage message, string pureMessage);
        Task<bool> IsImageSpam(SocketUserMessage message, string proxyUrl);
        Task<string> UnparseShortenUrl(string message);
        Task<bool> IsScam(SocketUserMessage message, string pureMessage);
        Task<bool> IsPorn(string message);
        Task<bool> IsSelfBot(ulong id);
        ConcurrentQueue<KeyValuePair<SocketUserMessage, string>> GetSpamCheckQueues();
    }
    public class AntiSpamService : IAntiSpamService
    {
        private ConcurrentQueue<KeyValuePair<SocketUserMessage, string>> toCheck = new ConcurrentQueue<KeyValuePair<SocketUserMessage, string>>();
        public ConcurrentQueue<KeyValuePair<SocketUserMessage, string>> GetSpamCheckQueues()
        {
            return toCheck;
        }

        public async Task<string> UnparseShortenUrl(string message)
        {
            string r = message;
            try
            {
                await FetchAPIData();
                var urls = Data.Instance.ShortenUrl.Where(message.Contains);
                foreach (var url in urls)
                {
                    var regex = $"(https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")}|https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")})\\/\\S*";
                    var result = Regex.Match(message, regex);
                    if (result.Success)
                    {
                        HttpClient hc = new HttpClient();
                        var response = await hc.GetAsync(result.Value);
                        var destination = response.Headers.Location?.ToString();
                        if(string.IsNullOrEmpty(destination))
                        {
                            destination = response.RequestMessage.RequestUri.ToString();
                        }
                        r = message.Replace(result.Value, destination);
                    };
                    
                }
            }
            catch
            {

            }
            return r;
        }

        public async Task<bool> IsPorn(string message)
        {
            try
            {
                await FetchAPIData();
                var urls = Data.Instance.PornList.Where(message.Contains);
                if (urls.Count() > 0)
                {
                    foreach (var url in urls)
                    {
                        var result = Regex.Match(message, $"(https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")}|https?:\\/\\/(?:www\\.|(?!www)){url.Replace(".", "\\.")}|www\\.{url.Replace(".", "\\.")})");
                        if (result.Success)
                        {
                            await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Match]: " + url + "\n", Encoding.UTF8);
                            return true;
                        };
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Exception]: " + ex.ToString() + "\n", Encoding.UTF8);
                return false;
            }
        }
       

        public async Task<bool> IsScam(SocketUserMessage message, string pureMessage)
        {
            try
            {
                Match match = Regex.Match(pureMessage, @"(https:\/\/)?(www\.)?(((discord(app)?)?\.com\/invite)|((discord(app)?)?\.gg))\/(?<invite>.+)");
                if (match.Success)
                {
                    if (match.Groups.TryGetValue("invite", out Group inviteurl))
                    {
                        HttpClient client = new();
                        HttpResponseMessage result = await client.GetAsync("https://discord.com/api/v9/invites/" + inviteurl.Value.Split(" ").First());
                        result.EnsureSuccessStatusCode();
                        DiscordInvite di = JsonConvert.DeserializeObject<DiscordInvite>(await result.Content.ReadAsStringAsync());
                        result = await client.GetAsync("https://api.phish.gg/server?id=" + di.Guild.Id);
                        result.EnsureSuccessStatusCode();
                        PhishGG pg = JsonConvert.DeserializeObject<PhishGG>(await result.Content.ReadAsStringAsync());
                        return pg.Match;
                    }
                }
                await FetchAPIData();
                if (Data.Instance.ScamList.Any(x => x.Domains.Any(y => pureMessage.Contains(y + '/'))))
                {
                    if (pureMessage.Contains("https://cdn.discordapp.com/") || pureMessage.Contains("https://discord.com/") || pureMessage.Contains("https://discord.gift/"))
                    {
                        //cdn, not scam lol
                        return false;
                    }
                    return true;
                }
                match = Regex.Match(pureMessage, @"(https?:\/\/)?(www[.])?(telegram|t)\.me\/([a-zA-Z0-9_-]*)\/?$");
                return match.Success;
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[" + message.Channel + "][Exception]: " + ex.ToString() + "\n", Encoding.UTF8);
                toCheck.Append(new KeyValuePair<SocketUserMessage, string> (message, pureMessage));
                return false;
            }

        }


        public bool IsTextSpam(SocketUserMessage message, string pureMessage)
        {
            using (LiteDatabase db = new("Filename=save\\" + (message.Channel as SocketGuildChannel).Guild.Id + ".db;connection=shared"))
            {
                ILiteCollection<Message> col = db.GetCollection<Message>("spam");
                Message data = col.FindOne(x => x.UserId == message.Author.Id);
                bool mon = false;
                if (data == null)
                {
                    _ = col.Insert(new Message() { UserId = message.Author.Id, SameTimes = 0, Content = pureMessage, LastMessageTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() });
                }
                else
                {
                    if ((((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - data.LastMessageTime) <= 2)
                    {
                        data.SameTimes++;
                        mon = true;
                    }
                    if (data.Content == pureMessage && (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - data.LastMessageTime) <= 60)
                    {
                        data.SameTimes++;
                        mon = true;
                    }
                    if (!mon)
                    {
                        data.SameTimes = 0;
                    }
                    data.LastMessageTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
                    data.Content = pureMessage;
                    _ = col.Update(data);
                    if(data.SameTimes >= 9)
                    {
                        //block directly
                        throw new ArgumentException("Ban");
                    }
                    if (data.SameTimes >= 3)
                    {
                        return true;
                    }
                }
                string[] splits = pureMessage.Split("\n");
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
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Security]: Updating ScamList DB\n", Encoding.UTF8);
                //fetch scamlist
                HttpClient client = new();
                HttpResponseMessage result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/domain-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
                result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/suspicious-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
            }

            if (Data.Instance.PornList.Count < 1)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Security]: Updating PornList DB\n", Encoding.UTF8);
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

            if(Data.Instance.ShortenUrl.Count < 1)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Security]: Updating ShortenUrl DB\n", Encoding.UTF8);
                HttpClient client = new();
                HttpResponseMessage result = await client.GetAsync("https://raw.githubusercontent.com/MISP/misp-warninglists/main/lists/url-shortener/list.json");
                var json = await result.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<ShortenUrl>(json);
                foreach(var item in list.List)
                {
                    Data.Instance.ShortenUrl.Add(item);
                }
            }
        }

        public async Task<bool> IsSelfBot(ulong id)
        {
            if (Data.Instance.SelfBots.Count < 1)
            {
                HttpClient client = new();
                HttpResponseMessage result = await client.GetAsync("https://gist.githubusercontent.com/Dziurwa14/05db50c66e4dcc67d129838e1b9d739a/raw/212a162cc9c1141a6ec60cf35788e9e1b9fbe779/spy.pet%20accounts");
                var json = await result.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ulong>>(json);
                foreach (var item in list)
                {
                    Data.Instance.SelfBots.Add(item);
                }
            }
            return Data.Instance.SelfBots.Contains(id);
        }

        public async Task<bool> IsImageSpam(SocketUserMessage message, string url)
        {
            using (LiteDatabase db = new("Filename=save\\" + (message.Channel as SocketGuildChannel).Guild.Id + ".db;connection=shared"))
            {
                string md5;
                try
                {
                    using (HttpClient client = new())
                    {
                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var md5hasher = MD5.Create())
                            {
                                var hash = md5hasher.ComputeHash(stream);
                                md5 = Convert.ToBase64String(hash);
                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }
                ILiteCollection<Message> col = db.GetCollection<Message>("img");
                Message data = col.FindOne(x => x.UserId == message.Author.Id);
                bool mon = false;
                if (data == null)
                {
                    _ = col.Insert(new Message() { UserId = message.Author.Id, SameTimes = 0, Content = md5, LastMessageTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() });
                }
                else
                {
                    if ((((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - data.LastMessageTime) <= 2)
                    {
                        data.SameTimes++;
                        mon = true;
                    }
                    if (data.Content == md5 && (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - data.LastMessageTime) <= 60)
                    {
                        data.SameTimes++;
                        mon = true;
                    }
                    if (!mon)
                    {
                        data.SameTimes = 0;
                    }
                    data.LastMessageTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
                    data.Content = md5;
                    _ = col.Update(data);
                    if (data.SameTimes >= 9)
                    {
                        //block directly
                        throw new ArgumentException("Ban");
                    }
                    if (data.SameTimes >= 3)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    public class AntiSpamJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
            DiscordSocketClient client = scope.Resolve<DiscordSocketClient>();
            IAntiSpamService antiSpamService = scope.Resolve<IAntiSpamService>();
            var queue = antiSpamService.GetSpamCheckQueues();
            while(!queue.IsEmpty)
            {
                if(queue.TryDequeue(out var checkQueue))
                {
                    if(await antiSpamService.IsScam(checkQueue.Key, checkQueue.Value))
                    {
                        GuildEmote angry = client.Guilds.FirstOrDefault(x => x.Id == 783913792668041216).Emotes.Where(x => x.Name.Contains("angry")).Last();
                        _ = await checkQueue.Key.ReplyAsync("請唔好Spam！" + checkQueue.Key.Author.Mention + angry.ToString());
                        await checkQueue.Key.DeleteAsync();
                        return;
                    }
                    break;
                }
            }
        }
    }
}
