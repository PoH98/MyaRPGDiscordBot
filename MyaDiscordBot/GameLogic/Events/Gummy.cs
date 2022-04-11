using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Gummy : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player, bool isWall)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("zzZZZ (米亞已經訓著，所以無任何特別事件哦！)", ephemeral: true);
            }
            player.CurrentHP = player.HP;
            if (isWall)
            {
                return command.RespondAsync("你對著不可行走的區域原地踏步後，偶遇類似甘米的女神經過, 瞬間覺得自己身體恢復嗮, 精神奕奕！", ephemeral: true);
            }
            else
            {
                return command.RespondAsync("偶遇類似甘米的女神經過, 瞬間覺得自己身體恢復嗮, 精神奕奕！", ephemeral: true);
            }

        }
    }
}
