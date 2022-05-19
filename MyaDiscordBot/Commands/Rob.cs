using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Rob : ICommand
    {
        private readonly IPlayerService playerService;
        private readonly IBattleService battleService;
        public Rob(IPlayerService playerService, IBattleService battleService)
        {
            this.playerService = playerService;
            this.battleService = battleService;
        }
        public string Name => "rob";

        public string Description => "Rob other players but limit to your same rank";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().WithName("target").WithDescription("The victim you want to rob").WithType(ApplicationCommandOptionType.User).WithRequired(true)
        };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            if (client.CurrentUser.Id == ((SocketGuildUser)command.Data.Options.First().Value).Id)
            {
                player.CurrentHP = 1;
                await command.RespondAsync("你竟然打劫米亞？！米亞將你打成殘血！", ephemeral: true);
                playerService.SavePlayer(player);
                return;
            }
            if (player.Lv < 10)
            {
                await command.RespondAsync("你未到10級，所以未解鎖此功能哦！", ephemeral: true);
                return;
            }
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                await command.RespondAsync("你正在休息！無法進行任何探索或者戰鬥！", ephemeral: true);
                return;
            }
            if (DateTime.Compare(player.NextRob, DateTime.Now) > 0)
            {
                await command.RespondAsync("你剛剛先打劫完其他玩家被通緝緊，馬上又出手打劫會被逮捕坐監整世！", ephemeral: true);
                return;
            }
            if (!player.Bag.Any(x => x.IsEquiped && x.UseTimes == -1 && x.Type != ItemType.道具))
            {
                await command.RespondAsync("你搞笑咩？空手打劫當自己係成龍或者係李小龍？", ephemeral: true);
                return;
            }
            var rank = 0;
            if (player.Bag.Count > 0 && player.Bag.Where(x => x.IsEquiped && x.UseTimes == -1 && x.Type != ItemType.道具).Count() > 0)
            {
                rank = player.Bag.Where(x => x.IsEquiped && x.UseTimes == -1 && x.Type != ItemType.道具).Max(x => x.Rank);
            }
            var victim = playerService.LoadPlayer(((SocketGuildUser)command.Data.Options.First().Value).Id, (command.Channel as SocketGuildChannel).Guild.Id);
            if (victim.Lv < 10)
            {
                await command.RespondAsync("你搞笑咩？對個新手咁惡，想搞到依個遊戲無人玩？", ephemeral: true);
                return;
            }
            if (player.DiscordId == victim.DiscordId)
            {
                //???
                player.CurrentHP = 1;
                await command.RespondAsync("你打左自己一餐，完全唔知道你想點？", ephemeral: true);
                playerService.SavePlayer(player);
                return;
            }
            var victimRank = 0;
            if (victim.Bag.Count > 0 && victim.Bag.Where(x => x.IsEquiped && x.UseTimes == -1 && x.Type != ItemType.道具).Count() > 0)
            {
                victimRank = victim.Bag.Where(x => x.IsEquiped && x.UseTimes == -1 && x.Type != ItemType.道具).Max(x => x.Rank);
            }
            if (Enumerable.Range(victimRank - 1, 3).Contains(rank))
            {
                if (DateTime.Compare(victim.RobShield, DateTime.Now) > 0)
                {
                    await command.RespondAsync("你見到你想打劫的對象實在太窮，連底褲都破破爛爛就知道佢剛剛先卑其他人打劫完，決定放過佢一馬！", ephemeral: true);
                    return;
                }
                //can fight
                var result = battleService.Battle(victim, player);
                Random rnd = new Random();
                var percentage = rnd.Next(3, 6);
                if (result.IsVictory)
                {
                    //player can gain money
                    var gain = victim.Coin * percentage / 100;
                    player.Coin += gain;
                    victim.Coin -= gain;
                    victim.CurrentHP = victim.HP;
                    player.CurrentHP = player.HP;
                    victim.RobShield = DateTime.Now.AddHours(12);
                    player.NextRob = DateTime.Now.AddHours(8);
                    if (player.NextCommand > DateTime.Now)
                    {
                        player.NextCommand = player.NextCommand.AddMinutes(30);
                    }
                    else
                    {
                        player.NextCommand = DateTime.Now.AddMinutes(30);
                    }
                    if (victim.NextCommand > DateTime.Now)
                    {
                        victim.NextCommand = victim.NextCommand.AddMinutes(30);
                    }
                    else
                    {
                        victim.NextCommand = DateTime.Now.AddMinutes(30);
                    }
                    await command.RespondAsync("估唔到" + command.User.Mention + "竟然咁心狠手辣，打劫左" + ((SocketGuildUser)command.Data.Options.First().Value).Mention + "，獲得左受害者" + gain + "蚊！");
                }
                else
                {
                    //victim can gain money
                    var gain = player.Coin * percentage / 100;
                    player.Coin -= gain;
                    victim.Coin += gain;
                    player.CurrentHP = player.HP;
                    victim.CurrentHP = victim.HP;
                    player.RobShield = DateTime.Now.AddHours(8);
                    player.NextRob = DateTime.Now.AddHours(8);
                    victim.RobShield = DateTime.Now.AddHours(4);
                    if (player.NextCommand > DateTime.Now)
                    {
                        player.NextCommand = player.NextCommand.AddMinutes(30);
                    }
                    else
                    {
                        player.NextCommand = DateTime.Now.AddMinutes(30);
                    }
                    if (victim.NextCommand > DateTime.Now)
                    {
                        victim.NextCommand = victim.NextCommand.AddMinutes(30);
                    }
                    else
                    {
                        victim.NextCommand = DateTime.Now.AddMinutes(30);
                    }
                    await command.RespondAsync("估唔到" + command.User.Mention + "竟然咁心狠手辣，打劫左" + ((SocketGuildUser)command.Data.Options.First().Value).Mention + "，可惜被反殺無左" + gain + "蚊！");
                }
                playerService.SavePlayer(player);
                playerService.SavePlayer(victim);
            }
            else
            {
                await command.RespondAsync("對手不符合你當前的戰力，無法展開公平戰鬥！上帝出現係你面前阻止左你的邪念", ephemeral: true);
            }
        }
    }
}
