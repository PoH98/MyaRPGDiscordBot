using MyaDiscordBot.Models;
using Newtonsoft.Json;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IItemService
    {
        IEnumerable<Item> GetShopItem(Player player);
        Task SaveData();
    }
    public class ItemService : IItemService
    {
        private readonly Items items;
        public ItemService(Items items)
        {
            this.items = items;
        }

        public IEnumerable<Item> GetShopItem(Player player)
        {
            var userRank = player.Lv / 10f;
            if (userRank < 1)
            {
                userRank = 1;
            }
            return items.Where(x => x.Price > 0 && x.Rank <= userRank && (!player.Bag.Any(y => x.Id == y.Id) || x.Type == ItemType.道具));
        }

        public Task SaveData()
        {
            return File.WriteAllTextAsync("config\\items.json", JsonConvert.SerializeObject(items, Formatting.Indented));
        }
    }
}
