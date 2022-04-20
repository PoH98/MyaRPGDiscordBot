using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Boss : ICommand
    {
        private readonly IBossService bossService;
        public Boss(IBossService bossService)
        {
            this.bossService = bossService;
        }
        public string Name => "boss";

        public string Description => "Check how much enemy to defeat for spawning boss or fight existing boss";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var enemies = bossService.GetEnemy((command.Channel as SocketGuildChannel).Guild.Id);
            if (enemies.Count() > 0)
            {
                ComponentBuilder cb = new ComponentBuilder();
                foreach (var x in enemies)
                {
                    string el;
                    switch (x.Enemy.Element)
                    {
                        case Element.Light:
                            el = "光";
                            break;
                        case Element.Dark:
                            el = "暗";
                            break;
                        case Element.God:
                            el = "神";
                            break;
                        case Element.Fire:
                            el = "火";
                            break;
                        case Element.Water:
                            el = "水";
                            break;
                        case Element.Wind:
                            el = "風";
                            break;
                        case Element.Earth:
                            el = "土";
                            break;
                        default:
                            el = "無";
                            break;
                    }
                    cb.WithButton("[" + el + "] " + x.Enemy.Name + " - HP: " + x.Enemy.HP, "boss-" + x.Id);
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
