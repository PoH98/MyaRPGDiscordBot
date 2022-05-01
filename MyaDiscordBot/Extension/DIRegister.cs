using Autofac;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent;
using MyaDiscordBot.Commands;
using MyaDiscordBot.GameLogic.Events;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace MyaDiscordBot.Extension
{
    internal static class DIRegister
    {
        internal static ContainerBuilder RegisterMyaBotService(this ContainerBuilder builder)
        {
            builder.RegisterType<PlayerService>().As<IPlayerService>();
            builder.RegisterType<MapService>().As<IMapService>();
            builder.RegisterType<SpawnerService>().As<ISpawnerService>();
            builder.RegisterType<BattleService>().As<IBattleService>();
            builder.RegisterType<ItemService>().As<IItemService>();
            builder.RegisterType<EventService>().As<IEventService>();
            builder.RegisterType<SettingService>().As<ISettingService>();
            builder.RegisterType<BossService>().As<IBossService>();
            builder.RegisterType<AntiSpamService>().As<IAntiSpamService>();
            return builder;
        }

        internal static ContainerBuilder RegisterSettingsConfig(this ContainerBuilder builder, DiscordSocketClient _client)
        {
            var settings = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config\\appsettings.json"));
            builder.RegisterInstance<IConfiguration>(settings);
            //Register commands
            var commandHandler = new CommandHandler(_client, settings);
            commandHandler.InstallCommands();
            var items = new Items();
            foreach(var i in Directory.GetFiles("config\\Items", "*.json"))
            {
                var its = JsonConvert.DeserializeObject<Items>(File.ReadAllText(i));
                items.AddRange(its);
            }
            builder.RegisterInstance(items);
            var enemy = JsonConvert.DeserializeObject<Enemies>(File.ReadAllText("config\\enemy.json"));
            builder.RegisterInstance(enemy);
            var craft = JsonConvert.DeserializeObject<CraftTableList>(File.ReadAllText("config\\crafttable.json"));
            builder.RegisterInstance(craft);
            var resource = JsonConvert.DeserializeObject<Resources>(File.ReadAllText("config\\resource.json"));
            builder.RegisterInstance(resource);
            Console.WriteLine("Loaded " + items.Count + " items");
            Console.WriteLine("Loaded " + enemy.Count + " enemies");
            Console.WriteLine("Loaded " + craft.Count + " crafttable");
            Console.WriteLine("Loaded " + resource.Count + " resource");
            return builder;
        }

        internal static ContainerBuilder RegisterInterfaces(this ContainerBuilder builder)
        {
            var commands = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);
            var buttons = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IButtonHandler).IsAssignableFrom(p) && !p.IsInterface);
            var rndEvents = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IRandomEvent).IsAssignableFrom(p) && !p.IsInterface);
            foreach (var command in commands)
            {
                MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
                method = method.MakeGenericMethod(command);
                method.Invoke(builder, new object[1] { builder });
            }
            foreach (var button in buttons)
            {
                MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
                method = method.MakeGenericMethod(button);
                method.Invoke(builder, new object[1] { builder });
            }
            foreach (var e in rndEvents)
            {
                MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
                method = method.MakeGenericMethod(e);
                method.Invoke(builder, new object[1] { builder });
            }
            Console.WriteLine("Loaded " + commands.Count() + " commands");
            Console.WriteLine("Loaded " + buttons.Count() + " handlers");
            Console.WriteLine("Loaded " + rndEvents.Count() + " events");
            return builder;
        }
    }
}
