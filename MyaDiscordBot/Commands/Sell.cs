using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Sell : ICommand
    {
        private readonly IPlayerService playerService;
        public Sell(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "sell";

        public string Description => "Sell off all your unwanted equipments and gain fast money!";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1] { new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.Number).WithName("method").WithDescription("Choose your selling logic here!").AddChoice("No Ability and Weaker than what you equiped the most weakest!", 1).AddChoice("No Ability and All not equiped, no matter what rank", 2).AddChoice("Sell just 1 item", 3).WithRequired(true) };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            Player player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            List<ItemEquip> dump = new();
            switch ((double)command.Data.Options.First().Value)
            {
                case 1:
                    //check if have any equiped
                    if (player.Bag.Where(x => x.IsEquiped).Count() > 0)
                    {
                        int maxRank = player.Bag.Where(x => x.IsEquiped).Min(x => x.Rank);
                        foreach (ItemEquip i in player.Bag)
                        {
                            //not equip and rank is lower than current rank, except golden AK
                            if (!i.IsEquiped && i.Rank < maxRank && i.Id != Guid.Empty && i.UseTimes == -1 && i.Ability == Ability.None)
                            {
                                dump.Add(i);
                            }
                        }
                    }
                    //else we can only detect not equiped
                    else if (player.Bag.Count > 0)
                    {
                        //retain max lv and lower 1 lv items
                        int maxRank = player.Bag.Max(x => x.Rank) - 1;
                        foreach (ItemEquip i in player.Bag)
                        {
                            //not equip and rank is lower than current rank, except golden AK
                            if (!i.IsEquiped && i.Rank < maxRank && i.Id != Guid.Empty && i.UseTimes == -1 && i.Ability == Ability.None)
                            {
                                dump.Add(i);
                            }
                        }
                    }
                    //else nothing to sell, since player bag is empty
                    break;
                case 2:
                    foreach (ItemEquip i in player.Bag)
                    {
                        //not equip and rank is lower than current rank, except golden AK
                        if (!i.IsEquiped && i.Id != Guid.Empty && i.UseTimes == -1 && i.Ability == Ability.None)
                        {
                            dump.Add(i);
                        }
                    }
                    break;
                case 3:
                    ComponentBuilder builder = new();
                    _ = builder.WithButton("火", "sellType-fire");
                    _ = builder.WithButton("風", "sellType-wind");
                    _ = builder.WithButton("土", "sellType-earth");
                    _ = builder.WithButton("水", "sellType-water");
                    _ = builder.WithButton("光", "sellType-light");
                    _ = builder.WithButton("暗", "sellType-dark");
                    await command.RespondAsync("甘米唔耐煩咁等你從背包拿出想賣的道具！", components: builder.Build(), ephemeral: true);
                    return;
            }
            int earned = 0;
            foreach (ItemEquip d in dump)
            {
                _ = player.Bag.Remove(d);
                player.Coin += 3;
                earned++;
            }
            await command.RespondAsync("已經到甘米的當鋪出售左" + earned + "個無用裝備，獲得" + (earned * 3) + "的錢！", ephemeral: true);
            playerService.SavePlayer(player);
        }
    }
}
