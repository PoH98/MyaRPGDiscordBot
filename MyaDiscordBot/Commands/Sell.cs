﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Sell : ICommand
    {
        private readonly IPlayerService playerService;
        public Sell(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "sell";

        public string Description => "Sell off all your unwanted equipments and gain fast money!";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1] { new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.Number).WithName("method").WithDescription("Choose your selling logic here!").AddChoice("No Ability and Weaker than what you equiped the most weakest!", 1).AddChoice("No Ability and All not equiped, no matter what rank", 2).WithRequired(true) };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            var dump = new List<ItemEquip>();
            switch ((double)command.Data.Options.First().Value)
            {
                case 1:
                    var maxRank = player.Bag.Where(x => x.IsEquiped).Min(x => x.Rank);
                    foreach (var i in player.Bag)
                    {
                        //not equip and rank is lower than current rank, except golden AK
                        if (!i.IsEquiped && i.Rank < maxRank && i.Id != Guid.Empty && i.UseTimes == -1 && i.Ability == Ability.None)
                        {
                            dump.Add(i);
                        }
                    }
                    break;
                case 2:
                    foreach (var i in player.Bag)
                    {
                        //not equip and rank is lower than current rank, except golden AK
                        if (!i.IsEquiped && i.Id != Guid.Empty && i.UseTimes == -1 && i.Ability == Ability.None)
                        {
                            dump.Add(i);
                        }
                    }
                    break;
            }
            var earned = 0;
            foreach (var d in dump)
            {
                player.Bag.Remove(d);
                player.Coin += 3;
                earned++;
            }
            await command.RespondAsync("已經到甘米的當鋪出售左" + earned + "個無用裝備，獲得" + (earned * 3) + "的錢！", ephemeral: true);
            playerService.SavePlayer(player);
        }
    }
}