using Discord;
using Discord.WebSocket;
using MyaDiscordBot.SelectEvent.Base;

namespace MyaDiscordBot.SelectEvent
{
    public class MarketSellCountHandler : ISelectHandler
    {
        public bool CheckUsage(string command)
        {
            return command.StartsWith("marketSellCount-");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            try
            {
                string customId = message.Data.Values.First().Replace("marketSellCount-", "mrkSCPrice-");
                SelectMenuBuilder sb = new();
                _ = sb.WithCustomId(Guid.NewGuid().ToString());
                _ = sb.WithPlaceholder("Select an option");
                List<double> price = new();
                for (int x = 1; x <= 25; x++)
                {
                    price.Add(x * 5);
                }
                price.Reverse();
                foreach (double i in price)
                {
                    SelectMenuOptionBuilder smob = new();
                    _ = smob.WithLabel("單個賣價: " + i + "$");
                    _ = smob.WithValue(customId + "-" + i);
                    _ = sb.AddOption(smob);
                }
                _ = sb.WithMinValues(1);
                _ = sb.WithMaxValues(1);
                ComponentBuilder cb = new();
                _ = cb.WithSelectMenu(sb);
                await message.RespondAsync("請選擇你要賣的價錢：", components: cb.Build(), ephemeral: true);
            }
            catch (Exception ex)
            {
                await message.RespondAsync(ex.ToString());
            }

        }
    }
}
