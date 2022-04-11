using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Walk : ICommand
    {
        private readonly IPlayerService _playerService;
        private readonly IBattleService _battleService;
        private readonly IMapService _mapService;
        private readonly IEventService _eventService;
        public Walk(IPlayerService playerService, IBattleService battleService, IMapService mapService, IEventService eventService)
        {
            _playerService = playerService;
            _battleService = battleService;
            _mapService = mapService;
            _eventService = eventService;
        }
        public string Name => "walk";

        public string Description => "Walk around at this floor";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().WithName("direction").WithDescription("Walk Direction").WithRequired(true).WithType(ApplicationCommandOptionType.Integer).AddChoice("Front", 8).AddChoice("Back", 2).AddChoice("Left", 4).AddChoice("Right", 6)
        };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                await command.RespondAsync("你正在休息！無法進行任何探索或者戰鬥！", ephemeral: true);
                return;
            }
            if (player.CurrentHP < 5)
            {
                await command.RespondAsync("你已經身受重傷，無法行動，米亞建議建設米亞妙妙屋激情對話恢復生命值哦！", ephemeral: true);
                return;
            }
            var loc = new Coordinate() { X = player.Coordinate.X, Y = player.Coordinate.Y };
            var hitWall = false;
            var enemy = _playerService.Walk(player, (long)command.Data.Options.First().Value);
            if (player.Coordinate.X == loc.X && player.Coordinate.Y == loc.Y)
            {
                hitWall = true;
            }
            if (enemy != null)
            {
                if (enemy.IsBoss)
                {
                    Data.Instance.Boss.Add((command.Channel as SocketGuildChannel).Guild.Id, enemy);
                    await command.RespondAsync("Boss已經生成！請各位玩家準備消滅" + enemy.Name + "！！");
                    return;
                }
                var br = _battleService.Battle(enemy, player);
                if (br.IsVictory)
                {
                    player.Coin += 2;
                    player.Exp += 1;
                    player.KilledEnemies++;
                    player.TotalKilledEnemies++;
                    var item = _battleService.GetReward(enemy, player);
                    if (item == null)
                    {
                        if (hitWall)
                        {
                            await command.RespondAsync("你對著個不能行走的區域原地踏步左一陣後，遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$！", ephemeral: true);
                        }
                        else
                        {
                            await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$！", ephemeral: true);
                        }

                    }
                    else
                    {
                        if (_playerService.AddItem(player, item))
                        {
                            if (hitWall)
                            {
                                await command.RespondAsync("你對著個不能行走的區域原地踏步左一陣後，遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$再額外獲得" + item.Name + "*1！", ephemeral: true);
                            }
                            else
                            {
                                await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$再額外獲得" + item.Name + "*1！", ephemeral: true);
                            }

                        }
                        else
                        {
                            if (hitWall)
                            {
                                //add failed, ignore and nothing happens
                                await command.RespondAsync("你對著個不能行走的區域原地踏步左一陣後，遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$！", ephemeral: true);
                            }
                            else
                            {
                                //add failed, ignore and nothing happens
                                await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$！", ephemeral: true);
                            }

                        }
                    }
                    await _mapService.KilledEnemy(player.ServerId);
                }
                else
                {
                    await command.RespondWithFileAsync("Assets\\wasted.png", "wasted.png", "你已死亡，請等待米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
                }
            }
            else
            {
                var @event = _eventService.GetRandomEvent();
                await @event.Response(command, player, hitWall);
            }
            _playerService.SavePlayer(player);
        }
    }
}
