using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Ben : IRandomEvent
    {
        Task IRandomEvent.Response(SocketSlashCommand command, Player player)
        {
            return DateTime.Now.Hour is < 6 and > 0
                ? command.RespondAsync("你見到夜空中閃過個機械人，佢同你揮手然後dum左一張紙條，上面寫著佢叫Ben，個機械人名叫高達完美超卓艾斯亞，你覺得一頭霧水...", ephemeral: true)
                : command.RespondAsync("當你向住目的地前進時，突然見到一部超級巨大嘅機械人喺你面前降落，跟住一個右手帶有燒傷疤痕嘅人離開機體 佢話佢係Ben 而後面嗰部就係高達完美超卓艾斯亞。你同米亞都一樣一頭霧水但自稱係Ben的人已經返左佢個機械人走左...", ephemeral: true);
        }
    }
}
