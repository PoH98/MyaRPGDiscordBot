using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Rainbow : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在路上突然見到有好幾個大字出現係空中，然後就係接二連三的從紅色到橙色，黃色，綠色等等的字慢慢湧現順著落去，當你睇得正興奮就要出現彩虹的時候突然出現個紅色的大字將彩虹斷左？！", ephemeral: true);
        }
    }
}
