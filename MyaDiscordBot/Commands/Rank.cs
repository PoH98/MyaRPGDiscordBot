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
            var players = playerService.GetPlayers((command.Channel as SocketGuildChannel).Guild.Id).Where(x => GetRank(x.Lv) == GetRank(player.Lv) && x.DiscordId != 0);
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("同你差唔多戰力的玩家富豪排行");
            embedBuilder.WithColor(Color.Green);
            var guild = client.GetGuild((command.Channel as SocketGuildChannel).Guild.Id);
            var currentIndex = 0;
            foreach (var p in players.OrderByDescending(x => x.Coin))
            {
                if (p.DiscordId == client.CurrentUser.Id)
                {
                    continue;
                }
                var name = "";
                var user = guild.GetUser(p.DiscordId);
                if (user == null)
                {
                    if(string.IsNullOrEmpty(p.Name))
                    {
                        continue;
                    }
                    name = p.Name;
                }
                else
                {
                    name = user.DisplayName;
                }
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                embedBuilder.AddField(name, "當前擁有" + p.Coin + "$");
                currentIndex++;
                if (currentIndex >= 20)
                {
                    break;
                }
            }
            await command.RespondAsync("", embed: embedBuilder.Build(), ephemeral: true);
        }

        private int GetRank(int lv)
        {
            var r = lv / 10;
            return r;
        }
    }
}
