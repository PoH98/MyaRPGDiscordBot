using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Search : ICommand
    {
        private readonly IPlayerService _playerService;
        private readonly IBattleService _battleService;
        private readonly IEventService _eventService;
        private readonly IBossService _bossService;
        private readonly IItemService _itemService;
        public Search(IPlayerService playerService, IBattleService battleService, IBossService bossService, IEventService eventService, IItemService itemService)
        {
            _playerService = playerService;
            _battleService = battleService;
            _eventService = eventService;
            _bossService = bossService;
            _itemService = itemService;
        }
        public string Name => "search";

        public string Description => "Search around in your current location";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            GetOption()
        };

        private SlashCommandOptionBuilder GetOption()
        {
            SlashCommandOptionBuilder op = new SlashCommandOptionBuilder().WithName("field").WithDescription("The place you want to go").WithRequired(true).WithType(ApplicationCommandOptionType.Integer);
            foreach (Element e in Enum.GetValues(typeof(Element)).Cast<Element>().Except(new List<Element>() { Element.Dark, Element.God, Element.Light }))
            {
                _ = op.AddChoice(e.ToString(), (int)e);
            }
            return op;
        }

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            Player player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
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
            Enemy enemy = _playerService.Walk(player, (Element)Convert.ToInt32(command.Data.Options.First().Value), BattleType.Default);
            if (enemy != null)
            {
                if (enemy.IsBoss)
                {
                    _bossService.AddBoss((command.Channel as SocketGuildChannel).Guild.Id, enemy);
                    await command.RespondAsync("野外Boss已經出現！請各位玩家準備消滅" + enemy.Name + "！！");
                    return;
                }
                BattleResult br = _battleService.Battle(enemy, player);
                if (br.IsVictory)
                {
                    int gain = Convert.ToInt32(Math.Round(1 + (1 + (player.Lv / 10f))));
                    player.Coin += gain;
                    _playerService.AddExp(player, 1);
                    Item item = _itemService.GetReward(enemy, player);
                    if (item == null)
                    {
                        await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到" + gain + "$！", ephemeral: true);
                    }
                    else
                    {
                        if (_playerService.AddItem(player, item))
                        {
                            await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到" + gain + "$再額外獲得" + item.Name + "*1！", ephemeral: true);
                        }
                        else
                        {
                            await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到" + gain + "$！", ephemeral: true);
                        }
                    }
                }
                else
                {
                    player.CurrentHP = player.HP;
                    await command.RespondWithFileAsync("Assets\\wasted.png", "wasted.png", "你已死亡，請等待米亞呼叫來的醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
                }
                player.LastCommand = DateTime.Now;
            }
            else
            {
                GameLogic.Events.Base.IRandomEvent @event = _eventService.GetRandomEvent();
                await @event.Response(command, player);
            }
            _playerService.SavePlayer(player);
        }
    }
}
