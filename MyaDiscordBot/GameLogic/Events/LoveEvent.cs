using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class LoveEvent : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("你背著訓著的米亞，突然跌左個信出來。發現原來係米亞寫卑你的表白信？！你當無睇到就咁留底封信係地下走佬左！", ephemeral: true);
            }
            player.CurrentHP = player.HP;
            return command.RespondAsync("米亞似乎神神秘秘咁唔知道佢想點，突然跳到你面前送左你一封信，原來係其他人送比米亞有個朱古力的信？米亞講卑你食之後你就食左個朱古力，瞬間覺得精力充沛！", ephemeral: true);
        }
    }
}
