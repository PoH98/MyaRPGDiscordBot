using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class HaoHao : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.NextCommand = DateTime.Now.AddMinutes(30);
            player.CurrentHP += 10;
            if (player.CurrentHP > player.HP)
            {
                player.CurrentHP = player.HP;
            }
            return command.RespondAsync("你見到個類似法師的人坐係個樹下休息，似乎係到聽緊咩錄音機的歌咁，你決定順便一起休息又有歌可以聽，點知聽左整半個鐘先發現一直播同一首歌，你決定逃離依個地方！", ephemeral: true);
        }
    }
}
