using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class JC12 : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("在一個小路上面，你同米亞見到有12個人係到忙緊搬運好多貨物，似乎貨物係DD炸彈，其中一個人睇到你哋後就衝過來自我介紹佢叫JC，係12個兄弟中最細嗰個，如果想買DD炸彈就需要2萬$，你覺得太貴馬上拉著米亞走佬", ephemeral: true);
        }
    }
}
