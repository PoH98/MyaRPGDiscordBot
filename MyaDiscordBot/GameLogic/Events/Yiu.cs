using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Yiu : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.NextCommand = DateTime.Now.AddMinutes(10);
            return command.RespondAsync("你在路上遇見個路人，佢好熱情就衝左過來同你say Hi，你嚇到走都唔切！佢一直追著你並且係後面狂叫 \"Yiu! Yiu! Yiu!\"! 你跑左至少10條街先撇開左佢！", ephemeral: true);
        }
    }
}
