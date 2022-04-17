using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Cow : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("你見到有個書店開到咁夜都未關門，於是想入去睇下有無咩特別的書可以睇下，點知第一眼就見到米亞和牛標題的書...你覺得莫名其妙就離開左書店！", ephemeral: true);
            }
            return command.RespondAsync("米亞在路上見到隻牛就跑過去同牛屎玩，剛剛好個牛的主人見到後寫落左一本米亞和牛的小說，似乎大賣？！", ephemeral: true);
        }
    }
}
