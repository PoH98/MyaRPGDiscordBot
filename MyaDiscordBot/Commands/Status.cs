using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System.Text;

namespace MyaDiscordBot.Commands
{
    public class Status : ICommand
    {
        private readonly IPlayerService playerService;
        private readonly IConfiguration configuration;
        public Status(IPlayerService playerService, IConfiguration configuration)
        {
            this.playerService = playerService;
            this.configuration = configuration;
        }
        public string Name => "status";

        public string Description => "Check my status";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            Player player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            EmbedBuilder eb = new() { Color = Color.Blue };
            _ = eb.WithTitle("我的資料");
            _ = eb.AddField("血量", player.CurrentHP + "/" + player.HP);
            _ = eb.AddField("傷害", player.Atk);
            _ = eb.AddField("防禦", player.Def);
            _ = eb.AddField("院友卡餘額", player.Coin + "$");
            _ = eb.AddField("經驗值", player.Exp + "/" + configuration.LV[player.Lv.ToString()]);
            _ = eb.AddField("等級", player.Lv);
            if (player.MarriedUser > 0)
            {
                _ = eb.AddField("結婚對象", (await client.GetUserAsync(player.MarriedUser)).Mention);
                _ = eb.AddField("結婚時長", player.MarriedTime);
            }
            EmbedBuilder bag = new() { Color = Color.Green };
            EmbedBuilder bag2 = new() { Color = Color.Red };
            EmbedBuilder resourceBag = new() { Color = Color.Teal };
            _ = bag.WithTitle("我的背包 (已裝備)");
            _ = resourceBag.WithTitle("原料背包");
            IOrderedEnumerable<ItemEquip> items = player.Bag.OrderByDescending(x => x.Rank);
            if (items.Count() < 1)
            {
                _ = bag.WithDescription("你的背包空的哦！咩都無！");
                _ = bag2.WithDescription("你的背包空的哦！咩都無！");
            }
            else
            {
                StringBuilder sb = new();
                foreach (ItemEquip item in items.Where(x => x.IsEquiped))
                {
                    //infinite use
                    _ = item.ItemLeft == -1 ? sb.AppendLine(item.Name) : sb.AppendLine(item.Name + "\t剩" + item.ItemLeft);
                }
                _ = bag.WithDescription(sb.ToString());
                _ = sb.Clear();
                _ = bag2.WithTitle("我的背包 (未裝備)");
                foreach (ItemEquip item in items.Where(x => !x.IsEquiped))
                {
                    //infinite use
                    _ = item.ItemLeft == -1 ? sb.AppendLine(item.Name) : sb.AppendLine(item.Name + "\t剩" + item.ItemLeft);
                }
                _ = bag2.WithDescription(sb.ToString());
            }
            player.ResourceBag ??= new List<HoldedResource>();
            IOrderedEnumerable<HoldedResource> resource = player.ResourceBag.Where(x => x.Amount > 0).OrderBy(x => x.DropRate);
            if (resource.Count() > 0)
            {
                foreach (HoldedResource item in resource)
                {
                    _ = resourceBag.AddField(item.Name, item.Amount);
                }
            }
            else
            {
                _ = resourceBag.WithDescription("你無任何原料哦！");
            }
            string desc = "";
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                desc = "**下次可探險時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>**";
            }
            await command.RespondAsync(desc, embeds: new Embed[] { eb.Build(), bag.Build(), bag2.Build(), resourceBag.Build() }, ephemeral: true);
        }
    }
}
