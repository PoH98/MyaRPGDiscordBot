using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class INH : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.Coin -= 6;
            return command.RespondAsync("你見到個INH飛天火車服務，決定用依個飛天火車直接快速移動到新地方，支付左6蚊車費用於你同米亞！", ephemeral: true);
        }
    }
}
