using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Rain : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.CurrentHP += 10;
            if (player.CurrentHP > player.HP)
            {
                player.CurrentHP = player.HP;
            }
            player.NextCommand = DateTime.Now.AddMinutes(20);
            return command.RespondAsync("突然落大雨，你同米亞走左去附近的大家雨J旅館避雨，遇見左老闆，佢免費送左你同米亞一人一碗屎水後見雨未停，順便唔收錢請埋大家食一餐好野！HP + 10", ephemeral: true);
        }
    }
}
