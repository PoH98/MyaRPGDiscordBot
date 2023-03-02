using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Rabbi : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("「唔係恐怖嘅幽靈，而係想住喺你心靈嘅紡霊...」當你準備將你嘅心靈交畀紡靈拉比時，突然發現佢真係幽靈👻👻你嚇到暈咗，醒翻已經發現自己喺基地...", ephemeral: true);
        }
    }
}
