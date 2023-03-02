using Autofac;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using MyaDiscordBot.SelectEvent.Base;
using Newtonsoft.Json;
using System.Reflection;

namespace MyaDiscordBot.Extension
{
    internal static class DIRegister
    {
        internal static ContainerBuilder RegisterMyaBotService(this ContainerBuilder builder)
        {
            _ = builder.RegisterType<PlayerService>().As<IPlayerService>();
            _ = builder.RegisterType<MapService>().As<IMapService>();
            _ = builder.RegisterType<SpawnerService>().As<ISpawnerService>();
            _ = builder.RegisterType<BattleService>().As<IBattleService>();
            _ = builder.RegisterType<ItemService>().As<IItemService>();
            _ = builder.RegisterType<EventService>().As<IEventService>();
            _ = builder.RegisterType<SettingService>().As<ISettingService>();
            _ = builder.RegisterType<BossService>().As<IBossService>();
            _ = builder.RegisterType<AntiSpamService>().As<IAntiSpamService>();
            _ = builder.RegisterType<MarketService>().As<IMarketService>();
            _ = builder.RegisterType<ChatGPTService>().As<IChatGPTService>().SingleInstance();
            return builder;
        }

        internal static ContainerBuilder RegisterSettingsConfig(this ContainerBuilder builder, DiscordSocketClient _client)
        {
            Configuration settings = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config\\appsettings.json"));
            _ = builder.RegisterInstance<IConfiguration>(settings);
            //Register commands
            CommandHandler commandHandler = new(_client, settings);
            commandHandler.InstallCommands();
            Items items = new();
            foreach (string i in Directory.GetFiles("config\\Items", "*.json"))
            {
                Items its = JsonConvert.DeserializeObject<Items>(File.ReadAllText(i));
                items.AddRange(its);
            }
            _ = builder.RegisterInstance(items);
            Enemies enemy = JsonConvert.DeserializeObject<Enemies>(File.ReadAllText("config\\enemy.json"));
            _ = builder.RegisterInstance(enemy);
            CraftTableList craft = JsonConvert.DeserializeObject<CraftTableList>(File.ReadAllText("config\\crafttable.json"));
            _ = builder.RegisterInstance(craft);
            Resources resource = JsonConvert.DeserializeObject<Resources>(File.ReadAllText("config\\resource.json"));
            _ = builder.RegisterInstance(resource);
            Pets pets = JsonConvert.DeserializeObject<Pets>(File.ReadAllText("config\\pets.json"));
            _ = builder.RegisterInstance(pets);
            Console.WriteLine("Loaded " + items.Count + " items");
            Console.WriteLine("Loaded " + enemy.Count + " enemies");
            Console.WriteLine("Loaded " + craft.Count + " crafttable");
            Console.WriteLine("Loaded " + resource.Count + " resource");
            Console.WriteLine("Loaded " + pets.Count() + " pets");
            return builder;
        }

        internal static ContainerBuilder RegisterInterfaces(this ContainerBuilder builder)
        {
            IEnumerable<Type> commands = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);
            IEnumerable<Type> buttons = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IButtonHandler).IsAssignableFrom(p) && !p.IsInterface);
            IEnumerable<Type> selects = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ISelectHandler).IsAssignableFrom(p) && !p.IsInterface);
            IEnumerable<Type> rndEvents = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IRandomEvent).IsAssignableFrom(p) && !p.IsInterface);
            foreach (Type select in selects)
            {
                MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
                method = method.MakeGenericMethod(select);
                _ = method.Invoke(builder, new object[1] { builder });
            }
            foreach (Type command in commands)
            {
                MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
                method = method.MakeGenericMethod(command);
                _ = method.Invoke(builder, new object[1] { builder });
            }
            foreach (Type button in buttons)
            {
                MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
                method = method.MakeGenericMethod(button);
                _ = method.Invoke(builder, new object[1] { builder });
            }
            foreach (Type e in rndEvents)
            {
                MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
                method = method.MakeGenericMethod(e);
                _ = method.Invoke(builder, new object[1] { builder });
            }
            Console.WriteLine("Loaded " + commands.Count() + " commands");
            Console.WriteLine("Loaded " + buttons.Count() + " handlers");
            Console.WriteLine("Loaded " + rndEvents.Count() + " events");
            return builder;
        }
    }
}
