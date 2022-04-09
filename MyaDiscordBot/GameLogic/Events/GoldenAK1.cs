using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class GoldenAK1 : IRandomEvent
    {
        private readonly IPlayerService playerService;
        public GoldenAK1(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public Task Response(SocketSlashCommand command, Player player)
        {
            playerService.AddItem(player, new Item
            {
                Id = Guid.Empty,
                Name = "黃金AK",
                UseTimes = -1,
                Price = -1,
                Ability = Ability.None,
                Atk = -10,
                Def = -10
            });
            return command.RespondAsync("突然一個熊貓神出現係你面前，手上有一把黃金AK同一把鑽石AK，問你想要邊把，你選擇黃金AK後熊貓神似乎好開心咁卑左你黃金AK後消失左！", ephemeral: true);
        }
    }
}
