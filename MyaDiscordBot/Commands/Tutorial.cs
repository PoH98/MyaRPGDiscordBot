using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands
{
    public class Tutorial : ICommand
    {
        public string Name => "tutorial";

        public string Description => "Check some basic game data";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().AddChoice("Element", 1).AddChoice("Item", 2).AddChoice("Battle", 3).AddChoice("Level up", 4).WithName("tutorial").WithDescription("The tutorial docs you want to view").WithType(ApplicationCommandOptionType.Integer).WithRequired(true)
        };

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            switch (Convert.ToInt32(command.Data.Options.First().Value))
            {
                case 1:
                    return command.RespondAsync("屬性相剋：\n火 > 風 > 土 > 水 > 火\n光 > 暗 > 光\n神\n屬性相剋會造成1.5倍傷害哦！", ephemeral: true);
                case 2:
                    return command.RespondAsync("道具/裝備使用：\n你可以通過/search打怪獲得裝備或者去/shop買！\n裝備獲得後記得用/equip裝備\n裝備不能混合其他屬性，所有裝備必須要同一個屬性！", ephemeral: true);
                case 3:
                    return command.RespondAsync("戰鬥：\n戰鬥的時候會通過計算你當前裝備的武器，護甲，護盾等等與對方的相差\n發動指令的人會先攻\n戰鬥途中會自動使用補血道具！", ephemeral: true);
                case 4:
                    return command.RespondAsync("升級：\n每10級 = 1 個新rank\n每升級5次會獲得1個Skill Point可以升你當前的防禦/攻擊力\n達到一定數量的經驗值就會自動升級！", ephemeral: true);
                default:
                    return command.RespondAsync("???");
            }
        }
    }
}
