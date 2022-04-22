using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.ButtonEvent
{
    public class GoldenAKHandler : IButtonHandler
    {
        private IPlayerService playerService;
        public GoldenAKHandler(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            if (command.StartsWith("goldenAK"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            if (Data.Instance.CacheDisableResponse.Contains(message.Message.Id))
            {
                await message.RespondAsync("熊貓神已經消失！", ephemeral: true);
                return;
            }
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            Data.Instance.CacheDisableResponse.Add(message.Message.Id);
            if (playerService.AddItem(player, new Item
            {
                Id = Guid.Empty,
                Name = "黃金AK",
                UseTimes = -1,
                Price = -1,
                Ability = Ability.None,
                Atk = -10,
                Def = -10,
                Element = Element.God,
                Type = ItemType.武器
            }))
            {
                if (message.Data.CustomId == "goldenAK1")
                {
                    await message.RespondAsync("熊貓神覺得好開心，卑左你一把黃金AK後消失左！黃金AK GET！", ephemeral: true);
                }
                else
                {
                    await message.RespondAsync("熊貓神覺得唔開心，強硬卑左你一把黃金AK後消失左！黃金AK GET！", ephemeral: true);
                }
            }
            else
            {
                if (message.Data.CustomId == "goldenAK1")
                {
                    await message.RespondAsync("熊貓神覺得好開心，卑左你一把黃金AK後消失左！不過你手上已經有另一把黃金AK所以你將佢dum左", ephemeral: true);
                }
                else
                {
                    await message.RespondAsync("熊貓神覺得唔開心，強硬卑左你一把黃金AK後消失左！不過你手上已經有另一把黃金AK所以你將佢dum左", ephemeral: true);
                }
            }
            playerService.SavePlayer(player);
        }
    }
}
