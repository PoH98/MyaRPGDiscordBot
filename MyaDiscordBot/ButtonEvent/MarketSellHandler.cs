using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.ButtonEvent
{
    public class MarketSellHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        public MarketSellHandler(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("marketSell-");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            try
            {
                var id = message.Data.CustomId.Replace("marketSell-", "");
                var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
                var r = player.ResourceBag.First(x => x.Id.ToString() == id);
                if(r.Amount > 0)
                {
                    SelectMenuBuilder sb = new SelectMenuBuilder();
                    sb.WithPlaceholder("Select an option");
                    sb.WithMinValues(1);
                    sb.WithCustomId(Guid.NewGuid().ToString());
                    sb.WithMaxValues(1);
                    List<SelectMenuOptionBuilder> smo = new List<SelectMenuOptionBuilder>();
                    for (int x = 1; x <= Math.Min(r.Amount, 25); x++)
                    {
                        SelectMenuOptionBuilder smob = new SelectMenuOptionBuilder();
                        smob.WithLabel(x + "個");
                        smob.WithValue("marketSellCount-" + x + "-" + r.Id);
                        smob.WithDescription("出售的數量");
                        smo.Add(smob);
                    }
                    sb.Options = smo;
                    ComponentBuilder cb = new ComponentBuilder();
                    cb.WithSelectMenu(sb);
                    await message.RespondAsync("請選擇你要賣的數量", ephemeral: true, components: cb.Build());
                }
                else
                {
                    await message.RespondAsync("你出售左空氣，獲得 0$", ephemeral: true);
                }
            }
            catch(Exception ex)
            {
                await message.RespondAsync(ex.ToString());
            }

        }
    }
}
