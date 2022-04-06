using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IItemService
    {
        IEnumerable<Item> GetShopItem(Player player);
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
            if (player.Bag.Count > 0)
            {
                return items.Where(x => x.Price > 0 && x.Rank >= player.Bag.Max(x => x.Rank) && x.Rank <= player.Bag.Max(x => x.Rank) + 1);
            }
            return items.Where(x => x.Price > 0 && x.Rank == 1);
        }
    }
}
