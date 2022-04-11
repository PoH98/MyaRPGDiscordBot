using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Boss : ICommand
    {
        private readonly IMapService mapService;
        private readonly IBattleService battleService;
        private readonly IPlayerService playerService;
        public Boss(IMapService mapService, IBattleService battleService, IPlayerService playerService)
        {
            this.mapService = mapService;
            this.battleService = battleService;
            this.playerService = playerService;
        }
        public string Name => "boss";

        public string Description => "Check how much enemy to defeat for spawning boss or fight existing boss";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            if (Data.Instance.Boss.ContainsKey((command.Channel as SocketGuildChannel).Guild.Id))
            {
                //fight
                var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
                if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
                {
                    await command.RespondAsync("你正在休息！無法進行任何探索或者戰鬥！", ephemeral: true);
                    return;
                }
                var result = battleService.Battle(Data.Instance.Boss[(command.Channel as SocketGuildChannel).Guild.Id], player);
                var coin = result.DamageDealt / 10;
                player.Coin += coin;
                player.Exp += coin / 2;
                if (result.IsVictory)
                {
                    Data.Instance.Boss.Remove((command.Channel as SocketGuildChannel).Guild.Id);
                    await command.RespondAsync(command.User.Mention + "對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$!\n Boss已被擊殺！院友們自動被傳送到下一層！");
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
                    await mapService.NextStage((command.Channel as SocketGuildChannel).Guild.Id);
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
                    await command.RespondAsync(command.User.Mention + "對Boss造成左" + result.DamageDealt + "傷害，獲得" + coin + "$! 不過由於Boss實在太強大，你已經陣亡而且被米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>");
                }
                player.BossDamage += result.DamageDealt;
                playerService.SavePlayer(player);
            }
            else
            {
                await command.RespondAsync("當前地圖剩" + mapService.GetEnemyLeft((command.Channel as SocketGuildChannel).Guild.Id) + "個敵人後先會生成Boss!", ephemeral: true);
            }
        }
    }
}
