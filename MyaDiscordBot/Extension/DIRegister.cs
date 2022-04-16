﻿using Autofac;
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
            return builder;
        }

        internal static ContainerBuilder RegisterSettingsConfig(this ContainerBuilder builder, DiscordSocketClient _client)
        {
            var settings = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config\\appsettings.json"));
            builder.RegisterInstance<IConfiguration>(settings);
            //Register commands
            var commandHandler = new CommandHandler(_client, settings);
            commandHandler.InstallCommands();
            var items = JsonConvert.DeserializeObject<Items>(File.ReadAllText("config\\items.json"));
            builder.RegisterInstance<Items>(items);
            var enemy = JsonConvert.DeserializeObject<Enemies>(File.ReadAllText("config\\enemy.json"));
            builder.RegisterInstance<Enemies>(enemy);
            Console.WriteLine("Detected " + items.Count() + " Items");
            Console.WriteLine("Detected " + enemy.Count() + " Enemies");
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
            Console.WriteLine("Detected " + commands.Count() + " Commands");
            Console.WriteLine("Detected " + buttons.Count() + " Handlers");
            Console.WriteLine("Detected " + rndEvents.Count() + " Events");
            return builder;
        }
    }
}
