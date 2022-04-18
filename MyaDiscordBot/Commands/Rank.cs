using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Rank : ICommand
    {
        private IPlayerService playerService;
        public Rank(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "rank";

        public string Description => "Show the top 20 players in your rank with most money";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            var players = playerService.GetPlayers((command.Channel as SocketGuildChannel).Guild.Id).Where(x => GetRank(x.Lv) == GetRank(player.Lv));
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("富豪排行");
            embedBuilder.WithColor(Color.Green);
            var guild = client.GetGuild((command.Channel as SocketGuildChannel).Guild.Id);
            var take = Math.Min(players.Count(), 20);
            foreach (var p in players.OrderByDescending(x => x.Coin).Take(take))
            {
                if(p.DiscordId == 0)
                {
                    //bugged data
                    continue;
                }
                var user = guild.GetUser(p.DiscordId);
                embedBuilder.AddField(user.DisplayName, "當前擁有" + p.Coin + "$");
            }
            await command.RespondAsync("", embed: embedBuilder.Build(), ephemeral: true);
        }

        private int GetRank(int lv)
        {
            var r = lv / 10;
            if (r < 1)
            {
                r = 1;
            }
            return r;
        }
    }
}
