using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;

namespace MyaDiscordBot.ButtonEvent
{
    public class ShopElementHandler : IButtonHandler
    {
        public bool CheckUsage(string command)
        {
            return command.StartsWith("shopEle-");
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            string element = message.Data.CustomId.Replace("shopEle-", "");
            ComponentBuilder builder = new();
            _ = builder.WithButton("武器", "shopType-" + element + "-weapon").WithButton("護甲", "shopType-" + element + "-amor").WithButton("戒指", "shopType-" + element + "-ring").WithButton("頸鏈", "shopType-" + element + "-necklece").WithButton("鞋", "shopType-" + element + "-shoes").WithButton("護盾", "shopType-" + element + "-shield");
            return message.RespondAsync("OK~咁請問你想買咩類型的野？", ephemeral: true, components: builder.Build());
        }
    }
}
