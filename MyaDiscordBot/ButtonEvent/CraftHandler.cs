using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.ButtonEvent
{
    public class CraftHandler : IButtonHandler
    {
        private readonly CraftTableList craftTableList;
        private readonly Resources resources;
        private readonly IPlayerService playerService;
        private readonly IItemService itemService;
        public CraftHandler(CraftTableList craftTableList, Resources resources, IPlayerService playerService, IItemService itemService)
        {
            this.craftTableList = craftTableList;
            this.playerService = playerService;
            this.itemService = itemService;
            this.resources = resources;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("craft-");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            Player player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            string id = message.Data.CustomId.Replace("craft-", "");
            CraftTable craft = craftTableList.FirstOrDefault(x => x.Item.ToString() == id);
            if (craft == null)
            {
                //???
                await message.RespondAsync("完全唔知道你想craft咩？", ephemeral: true);
                return;
            }
            Item item = itemService.GetCraftItem().First(x => x.Id.ToString() == id);
            EmbedBuilder eb = new();
            _ = eb.WithColor(Color.Magenta);
            _ = eb.WithTitle(item.Name + "所缺少的合成材料：");
            bool craftable = true;
            foreach (RequiredResource i in craft.Resources)
            {
                player.ResourceBag ??= new List<HoldedResource>();
                HoldedResource res = player.ResourceBag.FirstOrDefault(x => x.Id == i.Id);
                if (res != null)
                {
                    if ((i.Amount - res.Amount) > 0)
                    {
                        craftable = false;
                        _ = eb.AddField(res.Name, i.Amount - res.Amount);
                    }
                }
                else
                {
                    craftable = false;
                    Resource r = resources.FirstOrDefault(x => x.Id == i.Id);
                    _ = eb.AddField(r.Name, i.Amount);
                }
            }
            if (craftable)
            {
                ComponentBuilder c = new();
                _ = c.WithButton("確認", "craftConfirm-" + id, ButtonStyle.Success);
                await message.RespondAsync("是否要合成此道具", ephemeral: true, components: c.Build());
                return;
            }
            await message.RespondAsync("", embed: eb.Build(), ephemeral: true);
        }
    }
}
