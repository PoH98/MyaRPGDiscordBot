using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.ButtonEvent
{
    public class EquipButtonHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        public EquipButtonHandler(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            if (command.StartsWith("equip-") || command.StartsWith("unequip-"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            if (message.Data.CustomId.StartsWith("unequip-"))
            {
                var item = player.Bag.Where(x => x.Name == message.Data.CustomId.Replace("unequip-", "")).First();
                item.IsEquiped = false;
                await message.RespondAsync("已經解除裝備" + item.Name + "！", ephemeral: true);
            }
            else
            {
                var item = player.Bag.Where(x => x.Name == message.Data.CustomId.Replace("equip-", "")).First();
                item.IsEquiped = true;
                await message.RespondAsync("已經成功裝備" + item.Name + "！", ephemeral: true);
            }
            playerService.SavePlayer(player);
        }
    }
}
