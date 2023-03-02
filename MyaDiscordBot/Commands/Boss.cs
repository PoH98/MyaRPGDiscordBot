using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Boss : ICommand
    {
        private readonly IBossService bossService;
        private readonly IPlayerService playerService;
        public Boss(IBossService bossService, IPlayerService playerService)
        {
            this.bossService = bossService;
            this.playerService = playerService;
        }
        public string Name => "boss";

        public string Description => "Check how much enemy to defeat for spawning boss or fight existing boss";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            IEnumerable<BossSpawned> enemies = bossService.GetEnemy((command.Channel as SocketGuildChannel).Guild.Id);
            if (enemies.Count() > 0)
            {
                Player player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
                ComponentBuilder cb = new();
                foreach (BossSpawned x in enemies.Where(x => x.Enemy.Stage >= ((player.Lv / 10) - 1)))
                {
                    string el = x.Enemy.Element switch
                    {
                        Element.Light => "光",
                        Element.Dark => "暗",
                        Element.God => "神",
                        Element.Fire => "火",
                        Element.Water => "水",
                        Element.Wind => "風",
                        Element.Earth => "土",
                        _ => "無",
                    };
                    _ = cb.WithButton("[" + el + "] " + x.Enemy.Name + " - HP: " + x.Enemy.HP, "boss-" + x.Id);
                }
                await command.RespondAsync("當前所有Boss等待被擊殺: ", components: cb.Build(), ephemeral: true);
            }
            else
            {
                await command.RespondAsync("無任何Boss哦！", ephemeral: true);
            }
        }
    }
}
