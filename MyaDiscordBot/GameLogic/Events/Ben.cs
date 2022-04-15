using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Ben : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.CurrentHP = 1;
                return command.RespondAsync("你突然見到空中似乎有火花同導彈飛來飛去，夜晚之中特別耀眼，似乎係倆個高達在戰鬥中！當你睇得入神的時候突然一個導彈向你飛來，你炸剩殘血！", ephemeral: true);
            }
            return command.RespondAsync("路上你同米亞見到有個變形金剛飛左落來，米亞大叫左一聲高達後個變形金剛似乎極度唔開心就咁飛走左，你完全唔知道要點同米亞解釋...", ephemeral: true);
        }
    }
}
