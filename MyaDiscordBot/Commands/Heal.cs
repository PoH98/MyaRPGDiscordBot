﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Heal : ICommand
    {
        private readonly IPlayerService playerService;
        public Heal(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "heal";

        public string Description => "Add back 10 HP to your current HP but not more than max HP";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            if (DateTime.Compare(player.NextCommand, DateTime.Now) > 0)
            {
                await command.RespondAsync("你已經正在休息！唔可以雙重休息哦！", ephemeral: true);
                return;
            }
            player.CurrentHP += 10;
            int wait = 30;
            if (player.CurrentHP > player.HP)
            {
                int extra = player.CurrentHP - player.HP;
                wait -= extra * 3;
                player.CurrentHP = player.HP;
            }
            player.NextCommand = DateTime.Now.AddMinutes(wait);
            if (player.CurrentHP > 4)
            {
                await command.RespondAsync("你在原地與米亞一起建設左帳篷開始休息，下次可探險時間為：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
            }
            else
            {
                await command.RespondAsync("你在原地與米亞一起建設左米亞妙妙屋並且開始激情對話(?，下次可探險時間為：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
            }
            playerService.SavePlayer(player);
        }
    }
}
