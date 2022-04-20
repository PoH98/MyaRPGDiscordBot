using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Michael : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.NextCommand = DateTime.Now.AddMinutes(20);
                return command.RespondAsync("你在路上見到個有個有燒傷過疤痕的小朋友，佢似乎正在休息烤米糕中。你決定要係到同個小朋友一起休息一陣再上路！", ephemeral: true);
            }
            player.Coin -= 8;
            player.CurrentHP += 10;
            if (player.CurrentHP > player.HP)
            {
                player.CurrentHP = player.HP;
            }
            return command.RespondAsync("米亞似乎突然問到食物的味道，就朝著味道來源衝過去，發現有個似乎有個有燒傷過疤痕的小朋友在賣米糕。你花費左8蚊買左4塊米糕後一起繼續踏上旅途！", ephemeral: true);
        }
    }
}
