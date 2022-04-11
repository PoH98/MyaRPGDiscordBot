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
                if (item.IsEquiped)
                {
                    item.IsEquiped = false;
                    if (item.UseTimes == -1)
                    {
                        player.HP -= item.HP;
                        player.Atk -= item.Atk;
                        player.Def -= item.Def;
                    }
                    await message.RespondAsync("已經解除裝備" + item.Name + "！", ephemeral: true);
                }
                else
                {
                    await message.RespondAsync("你都無裝備" + item.Name + "，如何解除裝備？", ephemeral: true);
                }
            }
            else
            {
                //equip
                var item = player.Bag.Where(x => x.Name.ToLower() == message.Data.CustomId.Replace("equip-", "").ToLower()).First();
                if (!item.IsEquiped)
                {
                    if (item.UseTimes == -1)
                    {
                        if (player.Bag.Any(x => x.IsEquiped && x.UseTimes == -1 && x.Element != item.Element))
                        {
                            //not allow to equip different elements
                            await message.RespondAsync("你已經裝備其他屬性的裝備，無法裝備當前依個裝備！請卸下你的當前裝備後再進行換裝！", ephemeral: true);
                            return;
                        }
                        if (player.Bag.Any(x => x.IsEquiped && x.UseTimes == -1 && x.Type == item.Type))
                        {
                            //not allow to equip same type equipments
                            await message.RespondAsync("你已經裝備" + item.Type.ToString() + "，無法裝備當前依個裝備！請卸下你的當前裝備後再進行換裝！", ephemeral: true);
                            return;
                        }
                        player.HP += item.HP;
                        player.Atk += item.Atk;
                        player.Def += item.Def;
                    }
                    item.IsEquiped = true;
                    await message.RespondAsync("已經成功裝備" + item.Name + "！", ephemeral: true);
                }
                else
                {
                    await message.RespondAsync("你已經裝備" + item.Name + "，唔需要再重新裝備！", ephemeral: true);
                }
            }
            if (player.Def > player.HighestDef)
            {
                player.HighestDef = player.Def;
            }
            if (player.Atk > player.HighestAtk)
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
