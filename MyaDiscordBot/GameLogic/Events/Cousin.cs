using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Cousin : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在路上見到個大電視，上面正在播放緊關於米亞的節目，似乎好有趣。但突然電視後面跳出個自稱自己係自肥hin的人跑過來介紹依個節目係佢搞的，你覺得佢係怪人就離開左。", ephemeral: true);
        }
    }
}
