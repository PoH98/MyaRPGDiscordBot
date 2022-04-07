﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Walk : ICommand
    {
        private readonly IPlayerService _playerService;
        private readonly IBattleService _battleService;
        public Walk(IPlayerService playerService, IBattleService battleService)
        {
            _playerService = playerService;
            _battleService = battleService;
        }
        public string Name => "walk";

        public string Description => "Walk around at this floor";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().WithName("direction").WithDescription("Walk Direction").WithRequired(true).WithType(ApplicationCommandOptionType.Integer).AddChoice("Front", 1).AddChoice("Back", 2).AddChoice("Left", 3).AddChoice("Right", 4)
        };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                await command.RespondAsync("你正在休息！無法進行任何探索或者戰鬥！", ephemeral: true);
                return;
            }
            var enemy = _playerService.Walk(player, (long)command.Data.Options.First().Value);
            if (enemy != null)
            {
                var br = _battleService.Battle(enemy, player);
                if (br.IsVictory)
                {
                    player.Coin += 2;
                    player.Exp += 1;
                    await command.RespondAsync("你遇見隻" + enemy.Name + "而且發生戰鬥，成功獲勝並且得到2金幣同1經驗值！", ephemeral: true);
                }
                else
                {
                    await command.RespondWithFileAsync("Assets\\wasted.png", "wasted.png", "你已死亡，請等待醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
                }
            }
            else
            {
                await command.RespondAsync("Player Coordinate now " + player.Coordinate.X + "," + player.Coordinate.Y + " and no enemy met, suppose went random event");
            }
            _playerService.SavePlayer(player);
        }
    }
}
