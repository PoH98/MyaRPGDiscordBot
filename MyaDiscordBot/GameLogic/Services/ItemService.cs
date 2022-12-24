using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Books;
using Newtonsoft.Json;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IItemService
    {
        IEnumerable<Item> GetShopItem(Player player);
        IEnumerable<Item> GetCraftItem();
        Task SaveData();
        Item GetReward(Enemy enemy, Player player);
        Book GetBook(Player player);
        Resource GetResource(Player player);
        Item CraftItem(Player player, string id);
        void CraftSkill(Player player);
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
            if (player.Bag == null)
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
                if (i != null)
                {
                    i.Amount -= c.Amount;
                }
            }
            return items.First(x => x.Id == craftTable.Item);
        }

        public void CraftSkill(Player player)
        {
            var craftTable = new CraftTable
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
            foreach (var c in craftTable.Resources)
            {
                var i = player.ResourceBag.FirstOrDefault(x => x.Id == c.Id);
                if (i != null)
                {
                    i.Amount -= c.Amount;
                }
            }
            player.SkillPoint++;
        }

        public Book GetBook(Player player)
        {
            Random rnd = new Random();
            Book baseBook;
            switch(rnd.Next(0, 10)) 
            {
                case 0:
                case 1:
                    baseBook = new Book() { BType = BookType.Atk, Name = "AV書碎片" };
                    break;
                case 2:
                case 3:
                    baseBook = new Book() { BType = BookType.Def, Name = "D Cup書碎片" };
                    break;
                case 4:
                case 5:
                    baseBook = new Book() { BType = BookType.HP, Name = "H漫畫碎片" };
                    break;
                default:
                    baseBook = null;
                    break;
            }
            if (baseBook != null)
            {
                if (player.Books.Any(x => x.BType == baseBook.BType))
                {
                    player.Books.First(x => x.BType == baseBook.BType).Amount++;
                }
                else
                {
                    baseBook.Amount++;
                    player.Books.Add(baseBook);
                }
            }
            return baseBook;
        }

        public IEnumerable<Item> GetCraftItem()
        {
            return items.Where(x => x.Craft);
        }

        public Resource GetResource(Player player)
        {
            if (player.ResourceBag == null)
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
            if (enemy.Name == "米講粗口亞")
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
                        var rewards = items.Where(x => x.Price < 0 && enemy.DropRank.Any(y => y == x.Rank) && enemy.Element == x.Element && !player.Bag.Any(y => y.Id == x.Id) && !x.Craft).ToList();
                        if (rewards.Any())
                        {
                            double cumulSum = 0;
                            int cnt = rewards.Count();
                            for (int slot = 0; slot < cnt; slot++)
                            {
                                cumulSum += items[slot].DropRate;
                                rewards[slot].DropRate = cumulSum;
                            }
                            double divSpot = rnd.NextDouble() * cumulSum;
                            var r = rewards.FirstOrDefault(i => i.DropRate >= divSpot);
                            if (r != null && enemy.Stage > 10)
                            {
                                //infinite loop
                                r.Atk *= enemy.Stage / 10;
                                r.Def *= enemy.Stage / 10;
                                r.HP *= enemy.Stage / 10;
                                r.Name += " EX" + enemy.Stage / 10;
                            }
                            return r;
                        }
                    }
                    else
                    {
                        var rewards = items.Where(x => x.Price < 0 && enemy.DropRank.Any(y => y == x.Rank) && (x.Element == Element.Light || x.Element == Element.Dark) && !player.Bag.Any(y => y.Id == x.Id) && !x.Craft).ToList();
                        if (rewards.Any())
                        {
                            double cumulSum = 0;
                            int cnt = rewards.Count();
                            for (int slot = 0; slot < cnt; slot++)
                            {
                                cumulSum += items[slot].DropRate;
                                rewards[slot].DropRate = cumulSum;
                            }
                            double divSpot = rnd.NextDouble() * cumulSum;
                            var r = rewards.FirstOrDefault(i => i.DropRate >= divSpot);
                            if (r != null && enemy.Stage > 10)
                            {
                                //infinite loop
                                r.Atk *= enemy.Stage / 10;
                                r.Def *= enemy.Stage / 10;
                                r.HP *= enemy.Stage / 10;
                                r.Name += " EX" + enemy.Stage / 10;
                            }
                            return r;
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
