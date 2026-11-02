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
        Task<bool> IsImageSpam(SocketUserMessage message, string proxyUrl, bool uploadedFile = false);
        Task<string> UnparseShortenUrl(string message);
        Task<bool> IsScam(SocketUserMessage message, string pureMessage);
        Task<bool> IsPorn(string message);
        Task<bool> IsSelfBot(ulong id);
        ConcurrentQueue<KeyValuePair<SocketUserMessage, string>> GetSpamCheckQueues();
    }
    public class AntiSpamService : IAntiSpamService
    {
        private static readonly HttpClient HttpClient = new();
        private static readonly Regex UrlRegex = new(@"(?<![\w@.-])(?:https?://|www\.)?(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z]{2,}(?::\d{1,5})?(?:[/?#][^\s<>]*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        private static readonly Regex DiscordInviteRegex = new(@"(https:\/\/)?(www\.)?(((discord(app)?)?\.com\/invite)|((discord(app)?)?\.gg))\/(?<invite>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex TelegramRegex = new(@"(https?:\/\/)?(www[.])?(telegram|t)\.me\/([a-zA-Z0-9_-]*)\/?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private ConcurrentQueue<KeyValuePair<SocketUserMessage, string>> toCheck = new ConcurrentQueue<KeyValuePair<SocketUserMessage, string>>();
        public ConcurrentQueue<KeyValuePair<SocketUserMessage, string>> GetSpamCheckQueues()
        {
            return toCheck;
        }

        public async Task<string> UnparseShortenUrl(string message)
        {
            var result = new StringBuilder(message);
            try
            {
                var matches = UrlRegex.Matches(message);
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    var match = matches[i];
                    var address = match.Value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || match.Value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                        ? match.Value : "https://" + match.Value;
                    if (!Uri.TryCreate(address, UriKind.Absolute, out var uri)) continue;
                    try
                    {
                        using var response = await HttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                        var destination = response.RequestMessage?.RequestUri;
                        if (destination != null && destination != uri)
                        {
                            result.Remove(match.Index, match.Length);
                            result.Insert(match.Index, destination.ToString());
                        }
                    }
                    catch (HttpRequestException) { }
                    catch (TaskCanceledException) { }
                }
            }
            catch
            {

            }
            return result.ToString();
        }

        public async Task<bool> IsPorn(string message)
        {
            try
            {
                if (!UrlRegex.IsMatch(message)) return false;
                await FetchAPIData(porn: true);
                var urls = Data.Instance.PornList.Where(message.Contains);
                if (urls.Count() > 0)
                {
                    foreach (var url in urls)
                    {
                        if (message.Contains("http://" + url) || message.Contains("https://" + url) ||
                            message.Contains("http://www." + url) || message.Contains("https://www." + url) ||
                            message.Contains("www." + url))
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
                Match match = DiscordInviteRegex.Match(pureMessage);
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
                await FetchAPIData(scam: true);
                if (Data.Instance.ScamList.Any(x => x.Domains.Any(y => pureMessage.Contains(y + '/'))))
                {
                    if (pureMessage.Contains("https://cdn.discordapp.com/") || pureMessage.Contains("https://discord.com/") || pureMessage.Contains("https://discord.gift/"))
                    {
                        //cdn, not scam lol
                        return false;
                    }
                    return true;
                }
                match = TelegramRegex.Match(pureMessage);
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

        private async Task FetchAPIData(bool scam = false, bool porn = false)
        {
            if (scam && Data.Instance.ScamList.Count < 1)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Security]: Updating ScamList DB\n", Encoding.UTF8);
                //fetch scamlist
                HttpClient client = new();
                HttpResponseMessage result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/domain-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
                result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/suspicious-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
            }

            if (porn && Data.Instance.PornList.Count < 1)
            {
                await File.AppendAllTextAsync("log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".log", "[Security]: Updating PornList DB\n", Encoding.UTF8);
                //fetch pornlist
                HttpClient client = new();
                var link = await GetPornTxt(client);
                HttpResponseMessage result = await client.GetAsync(link);
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

        private async Task<string> GetPornTxt(HttpClient client)
        {
            HttpResponseMessage result = await client.GetAsync("https://raw.githubusercontent.com/Bon-Appetit/porn-domains/refs/heads/main/meta.json");
            var json = await result.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PornMetaJson>(json);
            return data.Blocklist.RawUrl;
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

        public async Task<bool> IsImageSpam(SocketUserMessage message, string url, bool uploadedFile = false)
        {
            string md5;
            try
            {
                using var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var host = response.RequestMessage?.RequestUri?.Host;
                if (!uploadedFile && !string.Equals(host, "cdn.discordapp.com", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(host, "media.discordapp.net", StringComparison.OrdinalIgnoreCase) &&
                    response.Content.Headers.ContentType?.MediaType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) != true) return false;
                using var stream = await response.Content.ReadAsStreamAsync();
                using var md5hasher = MD5.Create();
                md5 = Convert.ToBase64String(await md5hasher.ComputeHashAsync(stream));
            }
            catch
            {
                return false;
            }
            using (LiteDatabase db = new("Filename=save\\" + (message.Channel as SocketGuildChannel).Guild.Id + ".db;connection=shared"))
            {
                ILiteCollection<Message> col = db.GetCollection<Message>("img");
                Message data = col.FindOne(x => x.UserId == message.Author.Id);
                bool mon = false;
                if (data == null)
                {
                    _ = col.Insert(new Message() { UserId = message.Author.Id, SameTimes = 0, Content = md5, LastMessageTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() });
                }
                else
                {
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


        public static IEnumerable<string> GetUrls(string message)
        {
            foreach (Match match in UrlRegex.Matches(message))
                yield return match.Value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || match.Value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                    ? match.Value : "https://" + match.Value;
        }
    }

    public class AntiSpamJob : IJob
    {
        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellation)
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
