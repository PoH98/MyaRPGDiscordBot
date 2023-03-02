using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;

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
                string id = message.Data.CustomId.Replace("marketSell-", "");
                Models.Player player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
                Models.HoldedResource r = player.ResourceBag.First(x => x.Id.ToString() == id);
                if (r.Amount > 0)
                {
                    SelectMenuBuilder sb = new();
                    _ = sb.WithPlaceholder("Select an option");
                    _ = sb.WithMinValues(1);
                    _ = sb.WithCustomId(Guid.NewGuid().ToString());
                    _ = sb.WithMaxValues(1);
                    List<SelectMenuOptionBuilder> smo = new();
                    for (int x = 1; x <= Math.Min(r.Amount, 25); x++)
                    {
                        SelectMenuOptionBuilder smob = new();
                        _ = smob.WithLabel(x + "個");
                        _ = smob.WithValue("marketSellCount-" + x + "-" + r.Id);
                        _ = smob.WithDescription("出售的數量");
                        smo.Add(smob);
                    }
                    sb.Options = smo;
                    ComponentBuilder cb = new();
                    _ = cb.WithSelectMenu(sb);
                    await message.RespondAsync("請選擇你要賣的數量", ephemeral: true, components: cb.Build());
                }
                else
                {
                    await message.RespondAsync("你出售左空氣，獲得 0$", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                await message.RespondAsync(ex.ToString());
            }

        }
    }
}
