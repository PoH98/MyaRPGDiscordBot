using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.ButtonEvent
{
    internal class BossFightHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        private readonly IBattleService battleService;
        private readonly IBossService bossService;
        public BossFightHandler(IPlayerService playerService, IBattleService battleService, IBossService bossService)
        {
            this.playerService = playerService;
            this.bossService = bossService;
            this.battleService = battleService;
        }
        public bool CheckUsage(string message)
        {
            if (message.StartsWith("boss-"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                await message.RespondAsync("你正在休息！無法進行任何探索或者戰鬥！", ephemeral: true);
                return;
            }
            var bosses = bossService.GetEnemy((message.Channel as SocketGuildChannel).Guild.Id);
            if (bosses.Count() < 1)
            {
                await message.RespondAsync("此Boss已經唔存在！無法展開戰鬥！", ephemeral: true);
                return;
            }
            var boss = bosses.FirstOrDefault(x => x.Id.ToString() == message.Data.CustomId.Replace("boss-", ""));
            var result = battleService.Battle(boss.Enemy, player);
            bossService.UpdateEnemy((message.Channel as SocketGuildChannel).Guild.Id, boss);
            var coin = result.DamageDealt / 10;
            if (coin < 10)
            {
                coin = 10;
            }
            if(coin > 100)
            {
                coin = 100;
            }
            player.Coin += coin;
            player.Exp += coin / 2;
            if (result.IsVictory)
            {
                bossService.DefeatedEnemy((message.Channel as SocketGuildChannel).Guild.Id, boss);
                await message.RespondAsync(message.User.Mention + "對" + boss.Enemy.Name + "造成左" + result.DamageDealt + "傷害，獲得" + coin + "$!\n Boss已被擊殺！");
                if (player.CurrentHP < (player.HP / 100 * 70))
                {
                    player.CurrentHP = player.HP / 100 * 70;
                }
                player.CurrentHP += player.HP;
                int wait = player.HP * 3;
                if (player.CurrentHP > player.HP)
                {
                    int extra = player.CurrentHP - player.HP;
                    wait -= extra * 3;
                    player.CurrentHP = player.HP;
                }
                player.NextCommand = DateTime.Now.AddMinutes(wait);
            }
            else
            {
                if (player.CurrentHP < 0)
                {
                    player.CurrentHP = 0;
                }
                player.CurrentHP += player.HP;
                int wait = player.HP * 3;
                if (player.CurrentHP > player.HP)
                {
                    int extra = player.CurrentHP - player.HP;
                    wait -= extra * 3;
                    player.CurrentHP = player.HP;
                }
                player.NextCommand = DateTime.Now.AddMinutes(wait);
                await message.RespondAsync("你對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$! 不過由於Boss實在太強大，你已經陣亡而且被米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
            }
            player.BossDamage += result.DamageDealt;
            playerService.SavePlayer(player);
        }
    }
}