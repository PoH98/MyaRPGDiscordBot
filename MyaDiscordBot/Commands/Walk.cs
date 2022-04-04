using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task Handler(SocketSlashCommand command)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            var enemy = _playerService.Walk(player, (long)command.Data.Options.First().Value);
            if(enemy != null)
            {
                var br = _battleService.Battle(enemy, player);
                if (br.IsVictory)
                {
                    player.Coin += 2;
                    player.Exp += 1;
                }
                await command.RespondAsync("Player Coordinate now " + player.Coordinate.X + "," + player.Coordinate.Y + " and met enemy " + enemy.Name + " and Dealt " + br.DamageDealt + "dmg, Received " + br.DamageReceived + "dmg");
            }
            else
            {
                await command.RespondAsync("Player Coordinate now " + player.Coordinate.X + "," + player.Coordinate.Y + " and no enemy met, suppose went random event");
            }
            _playerService.SavePlayer(player);
        }
    }
}
