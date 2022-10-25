using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.ButtonEvent
{
    public class CraftConfirmHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        private readonly IItemService itemService;
        public CraftConfirmHandler(IPlayerService playerService, IItemService itemService)
        {
            this.playerService = playerService;
            this.itemService = itemService;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("craftConfirm") || command.StartsWith("craftSkillConfirm");
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            if(message.Data.CustomId == "craftSkillConfirm")
            {
                itemService.CraftSkill(player);
                return message.RespondAsync("已成功製作技能點！", ephemeral: true);
            }
            else
            {
                var item = itemService.CraftItem(player, message.Data.CustomId.Replace("craftConfirm-", ""));
                if (item == null)
                {
                    return message.RespondAsync("你已經存在依個道具！", ephemeral: true);
                }
                if (playerService.AddItem(player, item))
                {
                    playerService.SavePlayer(player);
                    return message.RespondAsync("已成功製作" + item.Name, ephemeral: true);
                }
                else
                {
                    return message.RespondAsync("製作失敗！所使用的原料已經全數消失！", ephemeral: true);
                }
            }
        }
    }
}
