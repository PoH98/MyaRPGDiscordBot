using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Gugu : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("zzZZZ (米亞已經訓著，所以無任何特別事件哦！)", ephemeral: true);
            }
            return command.RespondAsync("「咕咕咕~」，突然有一pat雀屎跌咗落米亞個頭度，米亞為發洩完隻生吞咗咕咕鳥Σ( ° △ °|||)︴", ephemeral: true);
        }
    }
}
