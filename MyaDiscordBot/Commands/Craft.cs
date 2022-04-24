using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Commands
{
    public class Craft
    {
        private readonly IPlayerService playerService;
        public Craft(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "craft";

        public string Description => "Craft items";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().WithDescription("What you want to do with the blacksmith?").WithName("option").AddChoice("break down",1).AddChoice("craft",2).WithRequired(true).WithType(ApplicationCommandOptionType.Integer)
        };

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var i = Convert.ToInt32(command.Data.Options.First().Value);
            if(i == 1)
            {
                var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
                //分解
                ComponentBuilder cb = new ComponentBuilder();
                foreach(var item in player.Bag.Where(x => !x.IsEquiped && x.Type != ItemType.道具))
                {
                    cb.WithButton(item.Name, "break-" + item.Id);
                }
                return command.RespondAsync("請選擇要分解的道具！", components: cb.Build(), ephemeral: true);
            }
            else
            {
                return command.RespondAsync("請選擇你要合成的特殊裝備！", ephemeral: true);
            }
        }
    }
}
