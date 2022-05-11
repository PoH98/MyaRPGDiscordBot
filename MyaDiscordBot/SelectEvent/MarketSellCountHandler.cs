using Discord;
using Discord.WebSocket;

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
                var customId = message.Data.Values.First().Replace("marketSellCount-", "mrkSCPrice-");
                SelectMenuBuilder sb = new SelectMenuBuilder();
                sb.WithCustomId(Guid.NewGuid().ToString());
                sb.WithPlaceholder("Select an option");
                List<double> price = new List<double>();
                for(int x = 1; x <= 25; x++)
                {
                    price.Add(x * 5);
                }
                price.Reverse();
                foreach(var i in price)
                {
                    SelectMenuOptionBuilder smob = new SelectMenuOptionBuilder();
                    smob.WithLabel("單個賣價: " + i + "$");
                    smob.WithValue(customId + "-" + i);
                    sb.AddOption(smob);
                }
                sb.WithMinValues(1);
                sb.WithMaxValues(1);
                ComponentBuilder cb = new ComponentBuilder();
                cb.WithSelectMenu(sb);
                await message.RespondAsync("請選擇你要賣的價錢：", components: cb.Build(), ephemeral: true);
            }
            catch(Exception ex)
            {
                await message.RespondAsync(ex.ToString());
            }

        }
    }
}
