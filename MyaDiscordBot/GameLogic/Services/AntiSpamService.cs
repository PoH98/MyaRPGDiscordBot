using Discord.WebSocket;
using LiteDB;
using MyaDiscordBot.Models.SpamDetection;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Antiscam;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IAntiSpamService
    {
        bool IsSpam(SocketUserMessage message);
        Task<bool> IsScam(SocketUserMessage message);
    }
    public class AntiSpamService : IAntiSpamService
    {
        public async Task<bool> IsScam(SocketUserMessage message)
        {
            var match = Regex.Match(message.Content, @"(https:\/\/)?(www\.)?(((discord(app)?)?\.com\/invite)|((discord(app)?)?\.gg))\/(?<invite>.+)");
            if (match.Success)
            {
                if(match.Groups.TryGetValue("invite", out Group inviteurl))
                {
                    HttpClient client = new HttpClient();
                    var result = await client.GetAsync("https://discord.com/api/v9/invites/" + inviteurl.Value);
                    var di = JsonConvert.DeserializeObject<DiscordInvite>(await result.Content.ReadAsStringAsync());
                    result = await client.GetAsync("https://api.phish.gg/server?id=" + di.Guild.Id);
                    var pg = JsonConvert.DeserializeObject<PhishGG>(await result.Content.ReadAsStringAsync());
                    return pg.Match;
                }
            }
            if(Data.Instance.ScamList.Count < 1)
            {
                //fetch scamlist
                HttpClient client = new HttpClient();
                var result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/domain-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
                result = await client.GetAsync("https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/suspicious-list.json");
                Data.Instance.ScamList.Add(JsonConvert.DeserializeObject<AntiscamData>(await result.Content.ReadAsStringAsync()));
            }
            if(Data.Instance.ScamList.Any(x => x.Domains.Any(y => message.Content.Contains(y))))
            {
                if (message.Content.Contains("https://cdn.discordapp.com/"))
                {
                    //cdn, not scam lol
                    return false;
                }
                return true;
            }
            match = Regex.Match(message.Content, @"(https?:\/\/)?(www[.])?(telegram|t)\.me\/([a-zA-Z0-9_-]*)\/?$");
            return match.Success;
        }

        public bool IsSpam(SocketUserMessage message)
        {
            using (var db = new LiteDatabase("Filename=save\\" + (message.Channel as SocketGuildChannel).Guild.Id + ".db;connection=shared"))
            {
                var col = db.GetCollection<Message>("spam");
                var data = col.FindOne(x => x.Id == message.Author.Id);
                if (data == null)
                {
                    col.Insert(new Message() { Id = message.Author.Id, SameTimes = 0, Content = message.Content });
                }
                else
                {
                    if (data.Content == message.Content)
                    {
                        data.SameTimes++;
                    }
                    else
                    {
                        col.Update(new Message() { Id = message.Author.Id, SameTimes = 0, Content = message.Content });
                    }
                    if (data.SameTimes >= 3)
                    {
                        return true;
                    }
                }
                var splits = message.Content.Split("\n");
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
    }
}
