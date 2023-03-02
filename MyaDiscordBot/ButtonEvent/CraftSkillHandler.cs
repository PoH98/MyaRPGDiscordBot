using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.ButtonEvent
{
    internal class CraftSkillHandler : IButtonHandler
    {
        private readonly Resources resources;
        private readonly IPlayerService playerService;
        private readonly IItemService itemService;

        public CraftSkillHandler(Resources resources, IPlayerService playerService, IItemService itemService)
        {
            this.playerService = playerService;
            this.itemService = itemService;
            this.resources = resources;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("craftSkill");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            Player player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            CraftTable craft = new()
            {
                Resources = new List<RequiredResource>
                {
                    { new RequiredResource{ Id = Guid.Parse("00e89a10-8262-4f31-8cf7-31a723bb7bd3"), Amount = 100 } },
                    { new RequiredResource{ Id = Guid.Parse("d140fbb3-56a1-48c2-b5c6-609fda7547ae"), Amount = 50 } },
                    { new RequiredResource{ Id = Guid.Parse("83f20fba-59f2-4e47-9a27-51c88b17b595"), Amount = 25 } },
                    { new RequiredResource{ Id = Guid.Parse("fc00ff11-2105-41b5-a0b4-aa1907026d9a"), Amount = 100 } },
                    { new RequiredResource{ Id = Guid.Parse("841f7103-cc65-4e8f-a581-fbb2557081e9"), Amount = 50 } },
                    { new RequiredResource{ Id = Guid.Parse("b15a9b94-3b61-4129-b953-1b57da8d82bd"), Amount = 25 } },
                    { new RequiredResource{ Id = Guid.Parse("98839c41-08b6-48b4-8f35-1d5a2d863707"), Amount = 100 } },
                    { new RequiredResource{ Id = Guid.Parse("e777a929-ba08-4898-85db-3ddc095a3f44"), Amount = 50 } },
                    { new RequiredResource{ Id = Guid.Parse("392d421f-44ec-461f-ac0a-0c6ae5d0571a"), Amount = 25 } }
                }
            };
            EmbedBuilder eb = new();
            _ = eb.WithColor(Color.Magenta);
            _ = eb.WithTitle("合成技能點所缺少的合成材料：");
            bool craftable = true;
            foreach (RequiredResource i in craft.Resources)
            {
                player.ResourceBag ??= new List<HoldedResource>();
                HoldedResource res = player.ResourceBag.FirstOrDefault(x => x.Id == i.Id);
                if (res != null)
                {
                    if ((i.Amount - res.Amount) > 0)
                    {
                        craftable = false;
                        _ = eb.AddField(res.Name, i.Amount - res.Amount);
                    }
                }
                else
                {
                    craftable = false;
                    Resource r = resources.FirstOrDefault(x => x.Id == i.Id);
                    _ = eb.AddField(r.Name, i.Amount);
                }
            }
            if (craftable)
            {
                ComponentBuilder c = new();
                _ = c.WithButton("確認", "craftSkillConfirm", ButtonStyle.Success);
                await message.RespondAsync("是否要合成技能點？", ephemeral: true, components: c.Build());
                return;
            }
            await message.RespondAsync("", embed: eb.Build(), ephemeral: true);
        }
    }
}
