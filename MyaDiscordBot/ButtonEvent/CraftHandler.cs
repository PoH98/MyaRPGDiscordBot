using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System.Text;

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
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            var id = message.Data.CustomId.Replace("craft-", "");
            var craft = craftTableList.FirstOrDefault(x => x.Item.ToString() == id);
            if(craft == null)
            {
                //???
                await message.RespondAsync("完全唔知道你想craft咩？", ephemeral: true);
                return;
            }
            var item = itemService.GetCraftItem().First(x => x.Id.ToString() == id);
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Color.Magenta);
            eb.WithTitle(item.Name + "所需要的合成材料：");
            var craftable = true;
            foreach(var i in craft.Resources)
            {
                if (player.ResourceBag == null)
                {
                    player.ResourceBag = new List<HoldedResource>();
                }
                var res = player.ResourceBag.FirstOrDefault(x => x.Id == i.Id && x.Amount >= i.Amount);
                if (res != null)
                {
                    if((i.Amount - res.Amount) > 0)
                    {
                        craftable = false;
                        eb.AddField(res.Name, (i.Amount - res.Amount));
                    }
                }
                else
                {
                    //craftable = false;
                    var r = resources.FirstOrDefault(x => x.Id == i.Id);
                    eb.AddField(r.Name, i.Amount);
                }
            }
            if (craftable)
            {
                var c = new ComponentBuilder();
                c.WithButton("確認", "craftConfirm-" + id, ButtonStyle.Success);
                await message.RespondAsync("是否要合成此道具", ephemeral: true, components: c.Build());
                return;
            }
            await message.RespondAsync("", embed: eb.Build(), ephemeral: true);
        }
    }
}
