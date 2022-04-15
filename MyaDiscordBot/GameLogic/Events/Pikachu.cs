using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Pikachu : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.NextCommand = DateTime.Now.AddMinutes(45);
                return command.RespondAsync("夜深時分，你似乎有見到幻覺有個皮卡超出現係你面前，不過考慮到已經咁夜自己未訓，應該係幻覺來，點知剛剛經過就突然被電暈！", ephemeral: true);
            }
            return command.RespondAsync("米亞唔知道去左邊，突然一聲慘叫後，有隻皮卡超向你走左過來？！經過一系列讀心術後先知道原來米亞唔小心掂到個不知名魔法方塊變成左皮卡超！好彩無幾耐後米亞終於變返原狀", ephemeral: true);
        }
    }
}
