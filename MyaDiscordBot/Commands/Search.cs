using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Search : ICommand
    {
        private readonly IPlayerService _playerService;
        private readonly IBattleService _battleService;
        private readonly IMapService _mapService;
        private readonly IEventService _eventService;
        private readonly IBossService _bossService;
        public Search(IPlayerService playerService, IBattleService battleService, IMapService mapService, IBossService bossService, IEventService eventService)
        {
            _playerService = playerService;
            _battleService = battleService;
            _mapService = mapService;
            _eventService = eventService;
            _bossService = bossService;
        }
        public string Name => "search";

        public string Description => "Search around in your current location";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            GetOption()
        };

        private SlashCommandOptionBuilder GetOption()
        {
            var op = new SlashCommandOptionBuilder().WithName("field").WithDescription("The place you want to go").WithRequired(true).WithType(ApplicationCommandOptionType.Integer);
            foreach (var e in Enum.GetValues(typeof(Element)).Cast<Element>().Except(new List<Element>() { Element.Dark, Element.God, Element.Light }))
            {
                op.AddChoice(e.ToString(), (int)e);
            }
            return op;
        }

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
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
            var enemy = _playerService.Walk(player, (Element)Convert.ToInt32(command.Data.Options.First().Value));
            if (enemy != null)
            {
                if (enemy.IsBoss)
                {
                    _bossService.AddBoss((command.Channel as SocketGuildChannel).Guild.Id, enemy);
                    await command.RespondAsync("野外Boss已經出現！請各位玩家準備消滅" + enemy.Name + "！！");
                    return;
                }
                var br = _battleService.Battle(enemy, player);
                if (br.IsVictory)
                {
                    player.Coin += 2;
                    _playerService.AddExp(player, 1);
                    var item = _battleService.GetReward(enemy, player);
                    if (item == null)
                    {
                        await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$！", ephemeral: true);
                    }
                    else
                    {
                        if (_playerService.AddItem(player, item))
                        {
                            await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$再額外獲得" + item.Name + "*1！", ephemeral: true);
                        }
                        else
                        {
                            await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2$！", ephemeral: true);
                        }
                    }
                }
                else
                {
                    player.CurrentHP = player.HP;
                    await command.RespondWithFileAsync("Assets\\wasted.png", "wasted.png", "你已死亡，請等待米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
                }
            }
            else
            {
                var @event = _eventService.GetRandomEvent();
                await @event.Response(command, player);
            }
            _playerService.SavePlayer(player);
        }
    }
}
