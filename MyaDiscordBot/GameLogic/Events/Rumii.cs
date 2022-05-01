using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Rumii : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.CurrentHP += 10;
                if (player.CurrentHP > player.HP)
                {
                    player.CurrentHP = player.HP;
                }
                return command.RespondAsync("你喺路上遇到偶像Rumii，忍唔住衝前搵佢簽名係你個武器上面，佢簽完名後就走左，你覺得極度開心！HP恢復10點！", ephemeral: true);
            }
            player.CurrentHP = 1;
            return command.RespondAsync("你喺路上遇到偶像Rumii，忍唔住衝前搵佢簽名係你個武器上面，米亞馬上衝過來一腳踢飛你，你成功去到宇宙後先跌落來，剩殘血！", ephemeral: true);
        }
    }
}
