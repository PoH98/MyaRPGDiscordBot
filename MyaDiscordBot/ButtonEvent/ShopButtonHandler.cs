using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.ButtonEvent
{
    public class ShopButtonHandler : IButtonHandler
    {
        private readonly Items items;
        private readonly IPlayerService playerService;
        public ShopButtonHandler(Items items, IPlayerService playerService)
        {
            this.items = items;
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            if (command.StartsWith("shop-"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            try
            {
                var data = items.Where(x => x.Id.ToString() == message.Data.CustomId.Replace("shop-", ""));
                if (data.Count() < 1)
                {
                    await message.RespondAsync("小貓已經唔係到，你對著空氣買野好玩咩？", ephemeral: true);
                    return;
                }
                var selected = data.First();
                var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
                if (player.Coin < selected.Price)
                {
                    await message.RespondAsync("小貓用彩虹棍敲左你個頭後走左，留底一張紙條表示窮鬼唔好煩佢", ephemeral: true);
                    return;
                }
                player.Coin -= selected.Price;
                if(player.Bag.Any(x => x.Name == selected.Name))
                {
                    player.Bag.Where(x => x.Name == selected.Name).First().ItemLeft++;
                }
                else
                {
                    player.Bag.Add(new ItemEquip(selected));
                }
                await message.RespondAsync("購買成功！", ephemeral: true);
                playerService.SavePlayer(player);
            }
            catch (Exception ex)
            {
                await message.RespondAsync("小貓好似收到其他玩家緊急召喚突然在購買途中消失左，錯誤資料:" + ex.Message, ephemeral: true);
            }
        }
    }
}
