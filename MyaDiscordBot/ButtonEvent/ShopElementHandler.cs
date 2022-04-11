using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.ButtonEvent
{
    public class ShopElementHandler : IButtonHandler
    {
        public bool CheckUsage(string command)
        {
            if (command.StartsWith("shopEle-"))
            {
                return true;
            }
            return false;
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var element = message.Data.CustomId.Replace("shopEle-", "");
            var builder = new ComponentBuilder();
            builder.WithButton("武器", "shopType-" + element + "-weapon").WithButton("護甲", "shopType-" + element + "-amor").WithButton("戒指", "shopType-" + element + "-ring").WithButton("頸鏈", "shopType-" + element + "-necklece").WithButton("鞋", "shopType-" + element + "-shoes").WithButton("護盾", "shopType-" + element + "-shield").WithButton("消耗品", "shopType-" + element + "-none");
            return message.RespondAsync("OK~咁請問你想買咩類型的野？", ephemeral: true, components: builder.Build());
        }
    }
}
