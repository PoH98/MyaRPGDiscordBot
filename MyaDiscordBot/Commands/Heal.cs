using Discord;
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
            player.CurrentHP += 10;
            int wait = 30;
            if (player.CurrentHP > player.HP)
            {
                int extra = player.CurrentHP - player.HP;
                wait -= extra * 3;
                player.CurrentHP = player.HP;
            }
            player.NextCommand = DateTime.Now.AddMinutes(wait);
            await command.RespondAsync("你在原地與米亞一起建設左帳篷開始休息，下次可探險時間為：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>", ephemeral: true);
            playerService.SavePlayer(player);
        }
    }
}
