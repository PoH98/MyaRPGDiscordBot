using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
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
            string itemName = command.Data.Options.First().Value.ToString();
            Item item = items.FirstOrDefault(x => x.Name.Contains(itemName));
            if (item == null)
            {
                return command.RespondAsync("米亞dum左本字典卑你，講佢搵唔到你講咩野！", ephemeral: true);
            }
            EmbedBuilder eb = new();
            _ = eb.WithTitle("道具資料：");
            _ = eb.AddField("裝備名稱", item.Name);
            string el = item.Element switch
            {
                Element.Light => "光",
                Element.Dark => "暗",
                Element.God => "神",
                Element.Fire => "火",
                Element.Water => "水",
                Element.Wind => "風",
                Element.Earth => "土",
                _ => "無",
            };
            _ = eb.AddField("屬性", el);
            _ = eb.AddField("部位", item.Type);
            if (item.Atk > 0)
            {
                _ = eb.AddField("傷害", "+" + item.Atk);
            }
            else if (item.Atk < 0)
            {
                _ = eb.AddField("傷害", item.Atk);
            }
            if (item.Def > 0)
            {
                _ = eb.AddField("防禦", "+" + item.Def);
            }
            else if (item.Def < 0)
            {
                _ = eb.AddField("防禦", item.Def);
            }
            if (item.HP > 0)
            {
                _ = eb.AddField("血量", "+" + item.HP);
            }
            else if (item.HP < 0)
            {
                _ = eb.AddField("血量", item.HP);
            }
            if (item.Ability != Ability.None)
            {
                switch (item.Ability)
                {
                    case Ability.Critical:
                        _ = eb.AddField("技能", "暴擊: 發動幾率" + (item.AbilityRate * 100).ToString("#0.00") + "%");
                        break;
                    case Ability.Heal:
                        _ = eb.AddField("技能", "吸血: 每次攻擊恢復" + (item.AbilityRate * 100).ToString("#0.00") + "% 攻擊的血量");
                        break;
                    case Ability.Immune:
                        _ = eb.AddField("技能", "無敵: 發動幾率" + (item.AbilityRate * 100).ToString("#0.00") + "%");
                        break;
                    case Ability.CopyCat:
                        _ = eb.AddField("技能", "複製: 完全複製對手戒指的技能");
                        break;
                    case Ability.DebuffStates:
                        _ = eb.AddField("技能", "緩衝: 降低對手數值（攻擊，防禦）" + (item.AbilityRate * 100).ToString("#0.00") + "%");
                        break;
                    case Ability.Reflect:
                        _ = eb.AddField("技能", "反擊: 有" + (item.AbilityRate * 100).ToString("#0.00") + "%幾率完全反擊受到的傷害並且抵消自身受到的傷害");
                        break;
                    case Ability.DebuffSkill:
                        _ = eb.AddField("技能", "封印: 對方不能發動任何戒指效果");
                        break;
                }
            }
            _ = eb.WithColor(Color.Teal);
            return command.RespondAsync("", embed: eb.Build(), ephemeral: true);
        }
    }
}
