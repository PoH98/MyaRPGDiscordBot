using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class YK : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("路上你同米亞見到一個用緊關刀的人在同一堆怪物戰鬥，你正準備跑過去幫手的時候個人突然發動唔知道咩法術，所有怪物直接受到 1647萬 的傷害，你嚇到退後左好多步，男人睇左你一下就離開左，你先係佢背後睇到大大個VIP 777的字，雖然唔知道係咩意思....", ephemeral: true);
        }
    }
}
