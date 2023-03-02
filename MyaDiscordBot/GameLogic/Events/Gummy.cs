using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Gummy : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour is < 6 and > 0)
            {
                return command.RespondAsync("zzZZZ (米亞已經訓著，所以無任何特別事件哦！)", ephemeral: true);
            }
            player.CurrentHP = player.HP;
            if (player.CurrentHP > player.HP)
            {
                player.CurrentHP = player.HP;
            }
            return command.RespondAsync("偶遇類似甘米的女神經過, 瞬間覺得自己身體恢復嗮, 精神奕奕！", ephemeral: true);
        }
    }
}
