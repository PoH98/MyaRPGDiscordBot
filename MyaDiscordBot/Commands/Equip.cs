using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Equip : ICommand
    {
        private readonly IPlayerService playerService;
        public Equip(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "equip";

        public string Description => "Equip your items to be used in next battle";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            var builder = new ComponentBuilder();
            foreach (var i in player.Bag.Where(x => x.IsEquiped))
            {
                if (i.Type == ItemType.道具 && i.ItemLeft == 0)
                {
                    continue;
                }
                //Already have this thing
                builder.WithButton(i.Name + " - 解除裝備", "unequip-" + i.Name.ToLower(), ButtonStyle.Danger);
            }
            foreach (var i in player.Bag.Where(x => !x.IsEquiped))
            {
                if (i.Type == ItemType.道具 && i.ItemLeft == 0)
                {
                    continue;
                }
                //Already have this thing
                builder.WithButton(i.Name + " - 裝備", "equip-" + i.Name.ToLower(), ButtonStyle.Success);
            }
            await command.RespondAsync("裝備背包內的道具：", ephemeral: true, components: builder.Build());
        }
    }
}
