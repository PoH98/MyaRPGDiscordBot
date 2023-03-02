using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Rank : ICommand
    {
        private readonly IPlayerService playerService;
        public Rank(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "rank";

        public string Description => "Show the top 20 players in your rank with most money";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            Models.Player player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            IEnumerable<Models.Player> players = playerService.GetPlayers((command.Channel as SocketGuildChannel).Guild.Id).Where(x => GetRank(x.Lv) == GetRank(player.Lv) && x.DiscordId != 0);
            EmbedBuilder embedBuilder = new();
            _ = embedBuilder.WithTitle("同你差唔多戰力的玩家富豪排行");
            _ = embedBuilder.WithColor(Color.Green);
            SocketGuild guild = client.GetGuild((command.Channel as SocketGuildChannel).Guild.Id);
            int currentIndex = 0;
            foreach (Models.Player p in players.OrderByDescending(x => x.Coin))
            {
                if (p.DiscordId == client.CurrentUser.Id)
                {
                    continue;
                }
                string name = "";
                SocketGuildUser user = guild.GetUser(p.DiscordId);
                if (user == null)
                {
                    if (string.IsNullOrEmpty(p.Name))
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
                _ = embedBuilder.AddField(name, "當前擁有" + p.Coin + "$");
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
            int r = lv / 10;
            return r;
        }
    }
}
