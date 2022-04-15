using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Yoyo : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.NextCommand = DateTime.Now.AddMinutes(30);
            return command.RespondAsync("路上你睇到個搖搖好強的戰士用搖搖快速攻擊多個敵人，你同米亞坐底係個樹下野餐睇佢戰鬥直到佢走左為止", ephemeral: true);
        }
    }
}
