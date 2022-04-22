using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class CheckItem : ICommand
    {
        private readonly Items items;
        public CheckItem(Items items)
        {
            this.items = items;
        }
        public string Name => "checkitem";

        public string Description => "Check item data by name";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().WithName("name").WithDescription("Item name").WithRequired(true).WithType(ApplicationCommandOptionType.String)
        };

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var itemName = command.Data.Options.First().Value.ToString();
            var item = items.FirstOrDefault(x => x.Name.Contains(itemName));
            if (item == null)
            {
                return command.RespondAsync("米亞dum左本字典卑你，講佢搵唔到你講咩野！", ephemeral: true);
            }
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle("道具資料：");
            eb.AddField("裝備名稱", item.Name);
            string el;
            switch (item.Element)
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
            eb.AddField("屬性", el);
            eb.AddField("部位", item.Type);
            if (item.Atk > 0)
            {
                eb.AddField("傷害", "+" + item.Atk);
            }
            else if (item.Atk < 0)
            {
                eb.AddField("傷害", item.Atk);
            }
            if (item.Def > 0)
            {
                eb.AddField("防禦", "+" + item.Def);
            }
            else if (item.Def < 0)
            {
                eb.AddField("防禦", item.Def);
            }
            if (item.HP > 0)
            {
                eb.AddField("血量", "+" + item.HP);
            }
            else if (item.HP < 0)
            {
                eb.AddField("血量", item.HP);
            }
            eb.WithColor(Color.Teal);
            return command.RespondAsync("", embed: eb.Build(), ephemeral: true);
        }
    }
}
