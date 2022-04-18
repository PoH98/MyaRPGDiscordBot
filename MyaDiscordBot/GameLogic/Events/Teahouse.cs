using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Teahouse : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你見到一間茶屋係個角落到，發現佢上面寫著個開門時間，不過似乎過左整4個鐘都未開門，最後放棄左去其他地方休息更好！", ephemeral: true);
        }
    }
}
