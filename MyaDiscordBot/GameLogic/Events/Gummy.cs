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
                player.CurrentHP = player.HP;
                if (player.CurrentHP > player.HP)
                {
                    player.CurrentHP = player.HP;
                }
                return command.RespondAsync("你見到月亮上面出現左個女神，似乎係甘米，瞬間覺得自己身體恢復嗮，精神奕奕！", ephemeral: true);
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
