using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public interface IRandomEvent
    {
        void Response(Player player);
    }
}
