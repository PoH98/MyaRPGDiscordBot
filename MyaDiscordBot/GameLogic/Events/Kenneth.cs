using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Kenneth : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.CurrentHP += 10;
            if (player.CurrentHP > player.HP)
            {
                player.CurrentHP = player.HP;
            }
            player.NextCommand = DateTime.Now.AddMinutes(20);
            return DateTime.Now.Hour is < 6 and > 0
                ? command.RespondAsync("已經入夜，你想食宵夜的時候呼叫左Food Mya，等左無幾耐就見到個熊貓服裝叫Kenneth的外送員坐著個魔毯送到！你放底左訓著的米亞後好好咁享受左一餐美食！", ephemeral: true)
                : command.RespondAsync("你同米亞覺得肚餓，所以決定叫Food Mya，等左無幾耐後就見到個熊貓服裝叫Kenneth的外送員坐著個魔毯送到！你同米亞食完後都好好咁休息左一段時間！", ephemeral: true);
        }
    }
}
