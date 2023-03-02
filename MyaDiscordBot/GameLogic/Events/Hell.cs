using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Hell : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.CurrentHP = 1;
            return command.RespondAsync("你突然跌落個陷阱，眼前一黑見到充滿熔岩同魔鬼的地方。突然你見到有個長著倆個巨型角的人形生物戴著小型皇冠但似乎搬運緊野而且身邊有好幾個守衛的人向你行左過來對你指手畫腳一陣，你腳下出現一個巨型魔法陣後突然就穿越返到米亞身邊！只不過似乎依然受左重傷？", ephemeral: true);
        }
    }
}
