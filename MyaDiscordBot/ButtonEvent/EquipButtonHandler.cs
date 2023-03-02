using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using System.Text;

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
            return command.StartsWith("equip-") || command.StartsWith("unequip-");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            Models.Player player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            if (message.Data.CustomId.StartsWith("unequip-"))
            {
                IEnumerable<Models.ItemEquip> items = player.Bag.Where(x => x.Name.ToLower() == message.Data.CustomId.Replace("unequip-", "").ToLower());
                if (items.Count() > 0)
                {
                    Models.ItemEquip item = items.First();
                    if (item.IsEquiped)
                    {
                        item.IsEquiped = false;
                        if (item.UseTimes == -1)
                        {
                            player.HP -= item.HP;
                            player.Atk -= item.Atk;
                            player.Def -= item.Def;
                            if (player.CurrentHP > player.HP)
                            {
                                player.CurrentHP = player.HP;
                            }
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
                    await message.RespondAsync("你成功解除裝備空氣！", ephemeral: true);
                }
            }
            else
            {
                //equip
                IEnumerable<Models.ItemEquip> items = player.Bag.Where(x => x.Name.ToLower() == message.Data.CustomId.Replace("equip-", "").ToLower());
                if (items.Count() > 0)
                {
                    Models.ItemEquip item = items.First();
                    if (!item.IsEquiped)
                    {
                        StringBuilder sb = new();
                        if (item.UseTimes == -1 && item.Type != Models.ItemType.道具 && item.Type != Models.ItemType.指環)
                        {
                            if (player.Bag.Any(x => x.IsEquiped && x.UseTimes == -1 && x.Element != item.Element && x.Type != Models.ItemType.道具 && x.Type != Models.ItemType.指環))
                            {
                                //not allow to equip different elements
                                foreach (Models.ItemEquip i in player.Bag.Where(x => x.IsEquiped && x.UseTimes == -1 && x.Element != item.Element && x.Type != Models.ItemType.道具 && x.Type != Models.ItemType.指環))
                                {
                                    i.IsEquiped = false;
                                    if (i.UseTimes == -1)
                                    {
                                        player.HP -= i.HP;
                                        player.Atk -= i.Atk;
                                        player.Def -= i.Def;
                                    }
                                    _ = sb.AppendLine(i.Name + "已經因屬性唔同而自動被解除裝備！");
                                }
                            }
                            if (player.Bag.Any(x => x.IsEquiped && x.UseTimes == -1 && x.Type == item.Type && x.Type != Models.ItemType.道具))
                            {
                                //not allow to equip same type equipments
                                foreach (Models.ItemEquip i in player.Bag.Where(x => x.IsEquiped && x.UseTimes == -1 && x.Type == item.Type && x.Type != Models.ItemType.道具))
                                {
                                    i.IsEquiped = false;
                                    if (i.UseTimes == -1)
                                    {
                                        player.HP -= i.HP;
                                        player.Atk -= i.Atk;
                                        player.Def -= i.Def;
                                    }
                                    _ = sb.AppendLine(i.Name + "已經因有相同類型的裝備而自動被解除裝備！");
                                }
                            }
                            player.HP += item.HP;
                            player.Atk += item.Atk;
                            player.Def += item.Def;
                        }
                        else if (item.UseTimes == -1 && item.Type == Models.ItemType.指環)
                        {
                            if (player.Bag.Any(x => x.IsEquiped && x.UseTimes == -1 && x.Type == item.Type && x.Type != Models.ItemType.道具))
                            {
                                //not allow to equip same type equipments
                                foreach (Models.ItemEquip i in player.Bag.Where(x => x.IsEquiped && x.UseTimes == -1 && x.Type == item.Type && x.Type != Models.ItemType.道具))
                                {
                                    i.IsEquiped = false;
                                    if (i.UseTimes == -1)
                                    {
                                        player.HP -= i.HP;
                                        player.Atk -= i.Atk;
                                        player.Def -= i.Def;
                                    }
                                    _ = sb.AppendLine(i.Name + "已經因有相同類型的裝備而自動被解除裝備！");
                                }
                            }
                            player.HP += item.HP;
                            player.Atk += item.Atk;
                            player.Def += item.Def;
                        }
                        item.IsEquiped = true;
                        if (sb.Length > 0)
                        {
                            await message.RespondAsync(sb.ToString() + "\n已經成功裝備" + item.Name + "！", ephemeral: true);
                        }
                        else
                        {
                            await message.RespondAsync("已經成功裝備" + item.Name + "！", ephemeral: true);
                        }
                    }
                    else
                    {
                        await message.RespondAsync("你已經裝備" + item.Name + "，唔需要再重新裝備！", ephemeral: true);
                    }
                }
                else
                {
                    await message.RespondAsync("已經成功裝備空氣！", ephemeral: true);
                }
            }
            playerService.SavePlayer(player);
        }
    }
}