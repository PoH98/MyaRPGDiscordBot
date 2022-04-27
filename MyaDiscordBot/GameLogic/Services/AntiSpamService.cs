using Discord.WebSocket;
using LiteDB;
using MyaDiscordBot.Models.SpamDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IAntiSpamService
    {
        bool IsSpam(SocketUserMessage message);
    }
    public class AntiSpamService : IAntiSpamService
    {
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
                    if (data.SameTimes > 3)
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
