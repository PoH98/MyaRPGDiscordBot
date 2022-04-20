using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Yoyo : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.NextCommand = DateTime.Now.AddMinutes(20);
            return command.RespondAsync("路上你睇到個搖搖好強的戰士用搖搖快速攻擊多個敵人，你同米亞坐底係個樹下野餐睇佢戰鬥直到佢走左為止，個戰士臨走前你似乎見到佢個披風上面寫著好有型的\"阿反\"大字，應該係佢個名呱？", ephemeral: true);
        }
    }
}
