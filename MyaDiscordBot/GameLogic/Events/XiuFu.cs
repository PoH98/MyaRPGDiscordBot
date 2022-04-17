using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class XiuFu : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在酒店裡休息，突然聽到隔離房傳來\"小夫我要入嚟啦！\"的聲音，然後就係一堆Myabe叫聲令你頂唔順馬上退左個房間繼續冒險！", ephemeral: true);
        }
    }
}
