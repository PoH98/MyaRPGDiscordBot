using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.ButtonEvent
{
    public class EquipButtonHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        public EquipButtonHandler(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            if (command.StartsWith("equip-") || command.StartsWith("unequip-"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            if (message.Data.CustomId.StartsWith("unequip-"))
            {
                var item = player.Bag.Where(x => x.Name.ToLower() == message.Data.CustomId.Replace("unequip-", "").ToLower()).First();
                item.IsEquiped = false;
                if(item.UseTimes == -1)
                {
                    player.HP -= item.HP;
                    player.Atk -= item.Atk;
                    player.Def -= item.Def;
                }
                await message.RespondAsync("已經解除裝備" + item.Name + "！", ephemeral: true);
            }
            else
            {
                var item = player.Bag.Where(x => x.Name.ToLower() == message.Data.CustomId.Replace("equip-", "").ToLower()).First();
                item.IsEquiped = true;
                if (item.UseTimes == -1)
                {
                    player.HP += item.HP;
                    player.Atk += item.Atk;
                    player.Def += item.Def;
                }
                await message.RespondAsync("已經成功裝備" + item.Name + "！", ephemeral: true);
            }
            if(player.Def > player.HighestDef)
            {
                player.HighestDef = player.Def;
            }
            if(player.Atk > player.HighestAtk)
            {
                player.HighestAtk = player.Atk;
            }
            if (player.HP > player.HighestHP)
            {
                player.HighestHP = player.HP;
            }
            playerService.SavePlayer(player);
        }
    }
}
