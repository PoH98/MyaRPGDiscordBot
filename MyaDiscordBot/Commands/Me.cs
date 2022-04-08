using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using System.Text;

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
            StringBuilder sb = new StringBuilder("我的資料：\n當前血量：" + player.CurrentHP + "/" + player.HP + "\n傷害：" + player.Atk + "\n防禦：" + player.Def + "\n院友卡餘額：" + player.Coin + "$\n經驗值：" + player.Exp + "\n背包：\n");
            foreach (var item in player.Bag.OrderByDescending(x => x.Rank).Take(20))
            {
                //infinite use
                if (item.ItemLeft == -1)
                {
                    sb.AppendLine(item.Name + "\t" + (item.IsEquiped ? "已裝備" : "未裝備"));
                }
                else
                {
                    sb.AppendLine(item.Name + "\t" + (item.IsEquiped ? "已裝備" : "未裝備") + "\t剩" + item.ItemLeft);
                }
            }
            return command.RespondAsync(sb.ToString(), ephemeral: true);
        }
    }
}
