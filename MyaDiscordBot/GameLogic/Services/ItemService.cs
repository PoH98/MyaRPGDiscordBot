using MyaDiscordBot.Models;
using Newtonsoft.Json;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IItemService
    {
        IEnumerable<Item> GetShopItem(Player player);
        IEnumerable<Item> GetCraftItem();
        Task SaveData();
        Item GetReward(Enemy enemy, Player player);
        Resource GetResource(Player player);
        Item CraftItem(Player player, string id);
    }
    public class ItemService : IItemService
    {
        private readonly Items items;
        private readonly List<Resource> resources;
        private readonly CraftTableList craftTables;
        public ItemService(Items items, Resources resources, CraftTableList craftTables)
        {
            this.items = items;
            this.resources = resources.Select(item => (Resource)item.Clone()).ToList();
            this.craftTables = craftTables;
        }

        public Item CraftItem(Player player, string id)
        {
            var craftTable = craftTables.FirstOrDefault(x => x.Item.ToString() == id);
            if(player.Bag == null)
            {
                player.Bag = new List<ItemEquip>();
            }
            if (player.Bag.Any(y => y.Id == craftTable.Item))
            {
                return null;
            }
            foreach (var c in craftTable.Resources)
            {
                var i = player.ResourceBag.FirstOrDefault(x => x.Id == c.Id);
                if(i != null)
                {
                    i.Amount -= c.Amount;
                }
            }
            return items.First(x => x.Id == craftTable.Item);
        }

        public IEnumerable<Item> GetCraftItem()
        {
            return items.Where(x => x.Craft);
        }

        public Resource GetResource(Player player)
        {
            if(player.ResourceBag == null)
            {
                player.ResourceBag = new List<HoldedResource>();
            }
            Random rnd = new Random();
            double cumulSum = 0;
            int cnt = resources.Count();
            for (int slot = 0; slot < cnt; slot++)
            {
                cumulSum += items[slot].DropRate;
                resources[slot].DropRate = cumulSum;
            }
            double divSpot = rnd.NextDouble() * cumulSum;
            return resources.FirstOrDefault(i => i.DropRate >= divSpot);
        }

        public Item GetReward(Enemy enemy, Player player)
        {
            if(enemy.Name== "米講粗口亞")
            {
                return null;
            }
            else
            {
                Random rnd = new Random();
                decimal i = (decimal)rnd.NextDouble();
                if (i <= enemy.ItemDropRate)
                {
                    if (!enemy.IsBoss)
                    {
                        var reward = items.Where(x => x.Price < 0 && enemy.DropRank.Any(y => y == x.Rank) && enemy.Element == x.Element && !player.Bag.Any(y => y.Id == x.Id) && !x.Craft).ToList();
                        if (reward.Any())
                        {
                            double cumulSum = 0;
                            int cnt = reward.Count();
                            for (int slot = 0; slot < cnt; slot++)
                            {
                                cumulSum += items[slot].DropRate;
                                reward[slot].DropRate = cumulSum;
                            }
                            double divSpot = rnd.NextDouble() * cumulSum;
                            return reward.FirstOrDefault(i => i.DropRate >= divSpot);
                        }
                    }
                    else
                    {
                        var reward = items.Where(x => x.Price < 0 && enemy.DropRank.Any(y => y == x.Rank) && (x.Element == Element.Light || x.Element == Element.Dark) && !player.Bag.Any(y => y.Id == x.Id) && !x.Craft).ToList();
                        if (reward.Any())
                        {
                            double cumulSum = 0;
                            int cnt = reward.Count();
                            for (int slot = 0; slot < cnt; slot++)
                            {
                                cumulSum += items[slot].DropRate;
                                reward[slot].DropRate = cumulSum;
                            }
                            double divSpot = rnd.NextDouble() * cumulSum;
                            return reward.FirstOrDefault(i => i.DropRate >= divSpot);
                        }
                    }
                }
                return null;
            }

        }

        public IEnumerable<Item> GetShopItem(Player player)
        {
            var userRank = player.Lv / 10f;
            if (userRank < 1)
            {
                userRank = 1;
            }
            return items.Where(x => x.Price > 0 && x.Rank <= userRank && (!player.Bag.Any(y => x.Id == y.Id) || x.Type == ItemType.道具) && !x.Craft);
        }

        public async Task SaveData()
        {
            foreach (var t in items.GroupBy(x => x.Rank).ToDictionary(x => x.Key, x => x.ToList()))
            {
                await File.WriteAllTextAsync("config\\Items\\T" + t.Key + ".json", JsonConvert.SerializeObject(t.Value, Formatting.Indented));
            }
        }
    }
}
