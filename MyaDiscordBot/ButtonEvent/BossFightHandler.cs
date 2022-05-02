using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.ButtonEvent
{
    internal class BossFightHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        private readonly IBattleService battleService;
        private readonly IBossService bossService;
        private readonly IItemService itemService;
        private Random rnd = new Random();
        public BossFightHandler(IPlayerService playerService, IBattleService battleService, IBossService bossService, IItemService itemService)
        {
            this.playerService = playerService;
            this.bossService = bossService;
            this.battleService = battleService;
            this.itemService = itemService;
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
            if (coin > 100)
            {
                coin = 100;
            }
            player.Coin += coin;
            playerService.AddExp(player, coin / 5);
            if(boss.Enemy.Name == "米講粗口亞")
            {
                //recover 70% HP directly
                if (player.CurrentHP < (player.HP * 70 / 100))
                {
                    player.CurrentHP = player.HP * 70 / 100;
                }
                var healAmount = player.HP - player.CurrentHP;
                var wait = healAmount * 2;
                player.CurrentHP += healAmount;
                if (player.CurrentHP > player.HP)
                {
                    player.CurrentHP = player.HP;
                }
                if (wait > 25)
                {
                    wait = 25;
                }
                player.NextCommand = DateTime.Now.AddMinutes(wait);
                var resource = itemService.GetResource(player);
                if (playerService.AddResource(player, resource))
                {
                    if (result.IsVictory)
                    {
                        bossService.DefeatedEnemy((message.Channel as SocketGuildChannel).Guild.Id, boss);
                        await message.RespondAsync(message.User.Mention + "對" + boss.Enemy.Name + "造成左" + result.DamageDealt + "傷害，獲得" + coin + "$!\n Boss已被擊殺！恭喜額外獲得" + resource.Name + "！");
                    }
                    else
                    {
                        await message.RespondAsync("你對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$! 不過由於Boss實在太強大，你已經陣亡而且被米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>！恭喜額外獲得" + resource.Name + "！", ephemeral: true);
                    }
                }
                else
                {
                    if (result.IsVictory)
                    {
                        bossService.DefeatedEnemy((message.Channel as SocketGuildChannel).Guild.Id, boss);
                        await message.RespondAsync(message.User.Mention + "對" + boss.Enemy.Name + "造成左" + result.DamageDealt + "傷害，獲得" + coin + "$!\n Boss已被擊殺！");
                    }
                    else
                    {
                        await message.RespondAsync("你對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$! 不過由於Boss實在太強大，你已經陣亡而且被米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>！", ephemeral: true);
                    }
                }
            }
            else
            {
                //recover 70% HP directly
                if (player.CurrentHP < (player.HP * 70 / 100))
                {
                    player.CurrentHP = player.HP * 70 / 100;
                }
                var healAmount = player.HP - player.CurrentHP;
                var wait = healAmount * 2;
                player.CurrentHP += healAmount;
                if (player.CurrentHP > player.HP)
                {
                    player.CurrentHP = player.HP;
                }
                if (wait > 25)
                {
                    wait = 25;
                }
                player.NextCommand = DateTime.Now.AddMinutes(wait);
                if (result.IsVictory)
                {
                    var reward = itemService.GetReward(boss.Enemy, player);
                    bossService.DefeatedEnemy((message.Channel as SocketGuildChannel).Guild.Id, boss);
                    if (reward != null)
                    {
                        if (playerService.AddItem(player, reward))
                        {
                            await message.RespondAsync(message.User.Mention + "對" + boss.Enemy.Name + "造成左" + result.DamageDealt + "傷害，獲得" + coin + "$!\n Boss已被擊殺！恭喜額外獲得" + reward.Name + "！");
                        }
                        else
                        {
                            await message.RespondAsync(message.User.Mention + "對" + boss.Enemy.Name + "造成左" + result.DamageDealt + "傷害，獲得" + coin + "$!\n Boss已被擊殺！");
                        }
                    }
                    else
                    {
                        await message.RespondAsync(message.User.Mention + "對" + boss.Enemy.Name + "造成左" + result.DamageDealt + "傷害，獲得" + coin + "$!\n Boss已被擊殺！");
                    }
                }
                else
                {
                    var drop = rnd.NextDouble() < 0.1;
                    if (drop)
                    {
                        var resource = itemService.GetResource(player);
                        if (resource != null && playerService.AddResource(player, resource))
                        {
                            await message.RespondAsync("你對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$! 不過由於Boss實在太強大，你已經陣亡而且被米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>！恭喜額外獲得" + resource.Name + " * 1！", ephemeral: true);
                        }
                        else
                        {
                            await message.RespondAsync("你對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$! 不過由於Boss實在太強大，你已經陣亡而且被米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
                        }
                    }
                    else
                    {
                        await message.RespondAsync("你對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$! 不過由於Boss實在太強大，你已經陣亡而且被米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
                    }
                    
                }
            }

            player.BossDamage += result.DamageDealt;
            playerService.SavePlayer(player);
        }
    }
}