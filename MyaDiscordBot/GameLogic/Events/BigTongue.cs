using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class BigTongue : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在路上突然聽到有個古怪笑聲，令你覺得係咪有人中左咩笑死病。由於笑聲顯得好似要斷氣咁你決定要去睇一睇，點知行到一間木屋發現有個超大舌頭的怪獸係到笑，佢見到你之後笑得更慘然後就消失左。", ephemeral: true);
        }
    }
}
