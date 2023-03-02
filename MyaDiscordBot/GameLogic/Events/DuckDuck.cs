using Autofac;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class DuckDuck : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
            IItemService itemService = scope.Resolve<IItemService>();
            IEnumerable<Item> items = itemService.GetCraftItem();
            Random rnd = new();
            Item selectedItem = items.ToList()[rnd.Next(0, items.Count())];
            Resource resource = itemService.GetResource(player);
            return command.RespondAsync("你在路上見到一隻即將卑人燒的鴨，你同米亞衝上去買低隻鴨救左佢，隻鴨感激你送左你一堆唔知道有咩用的材料！\n獲得左" + resource.Name, ephemeral: true);
        }
    }
}
