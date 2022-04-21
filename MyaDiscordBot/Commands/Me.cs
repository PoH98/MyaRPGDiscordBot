using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System.Text;

namespace MyaDiscordBot.Commands
{
    public class Me : ICommand
    {
        private readonly IPlayerService playerService;
        private readonly IConfiguration configuration;
        public Me(IPlayerService playerService, IConfiguration configuration)
        {
            this.playerService = playerService;
            this.configuration = configuration;
        }
        public string Name => "me";

        public string Description => "Check my status";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id, command.User.Username);
            EmbedBuilder eb = new EmbedBuilder() { Color = Color.Blue };
            eb.WithTitle("我的資料");
            eb.AddField("血量", player.CurrentHP + "/" + player.HP);
            eb.AddField("傷害", player.Atk);
            eb.AddField("防禦", player.Def);
            eb.AddField("院友卡餘額", player.Coin + "$");
            eb.AddField("經驗值", player.Exp +"/"+ configuration.LV[player.Lv.ToString()]);
            eb.AddField("等級", player.Lv);
            EmbedBuilder bag = new EmbedBuilder() { Color = Color.Green };
            EmbedBuilder bag2 = new EmbedBuilder() { Color = Color.Red };
            bag.WithTitle("我的背包 (已裝備)");
            var items = player.Bag.OrderByDescending(x => x.Rank);
            if (items.Count() < 1)
            {
                bag.WithDescription("你的背包空的哦！咩都無！");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in items.Where(x => x.IsEquiped))
                {
                    //infinite use
                    if (item.ItemLeft == -1)
                    {
                        sb.AppendLine(item.Name);
                    }
                    else
                    {
                        sb.AppendLine(item.Name + "\t剩" + item.ItemLeft);
                    }
                }
                bag.WithDescription(sb.ToString());
                sb.Clear();
                bag2.WithTitle("我的背包 (未裝備)");
                foreach (var item in items.Where(x => !x.IsEquiped))
                {
                    //infinite use
                    if (item.ItemLeft == -1)
                    {
                        sb.AppendLine(item.Name);
                    }
                    else
                    {
                        sb.AppendLine(item.Name + "\t剩" + item.ItemLeft);
                    }
                }
                bag2.WithDescription(sb.ToString());
            }

            string desc = "";
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                desc = "**下次可探險時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>**";
            }
            return command.RespondAsync(desc, embeds: new Embed[] { eb.Build(), bag.Build(), bag2.Build() }, ephemeral: true);
        }
    }
}
