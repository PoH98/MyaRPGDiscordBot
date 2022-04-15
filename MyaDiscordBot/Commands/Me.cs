using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Me : ICommand
    {
        private readonly IPlayerService playerService;
        public Me(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "me";

        public string Description => "Check my status";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            EmbedBuilder eb = new EmbedBuilder() { Color = Color.Blue };
            eb.WithTitle("我的資料");
            eb.AddField("血量", player.CurrentHP + "/" + player.HP);
            eb.AddField("傷害", player.Atk);
            eb.AddField("防禦", player.Def);
            eb.AddField("院友卡餘額", player.Coin + "$");
            eb.AddField("經驗值", player.Exp);
            eb.AddField("等級", player.Lv);
            EmbedBuilder bag = new EmbedBuilder() { Color = Color.Blue };
            bag.WithTitle("我的背包");
            var items = player.Bag.OrderByDescending(x => x.Rank);
            if (items.Count() < 1)
            {
                bag.WithDescription("你的背包空的哦！咩都無！");
            }
            foreach (var item in items)
            {
                //infinite use
                if (item.ItemLeft == -1)
                {
                    bag.AddField(item.Name, item.IsEquiped ? "已裝備" : "未裝備");
                }
                else
                {
                    bag.AddField(item.Name, (item.IsEquiped ? "已裝備" : "未裝備") + "\t剩" + item.ItemLeft);
                }
            }
            string desc = "";
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                desc = "**下次可探險時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>**";
            }
            return command.RespondAsync(desc, embeds: new Embed[] { eb.Build(), bag.Build() }, ephemeral: true);
        }
    }
}
