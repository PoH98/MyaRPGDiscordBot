using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class HeySing : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.CurrentHP = 1;
                return command.RespondAsync("你在路上見到一個似乎正在休息的表演者，佢見到你後大叫\"Hey, Sing!\"，你唔知道點算好但對方似乎極度不滿意直接開啟左秘密武器發射至少上千個DD炸彈將你打成殘血！", ephemeral: true);
            }
            return command.RespondAsync("你到左一個城鎮廣場，似乎有人係到表演緊，米亞衝上去想睇你只好跟上去。表演者睇到你同米亞身影後突然指著你哋大叫\"Hey, Sing!\"，全場人望著你哋場面極度尷尬，好彩米亞就現場唱左一首叫咩\"院歌\"後大家似乎好滿意全場高呼度過難忘的一日！", ephemeral: true);
        }
    }
}
