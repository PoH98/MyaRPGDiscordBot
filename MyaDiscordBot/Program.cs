﻿// See https://aka.ms/new-console-template for more information
using Autofac;
using Autofac.Builder;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot;
using MyaDiscordBot.Commands;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using Newtonsoft.Json;
using System.Reflection;
//Create DC Bot Client//
var _client = new DiscordSocketClient();
_client.Log += Log;
var commandHandler = new CommandHandler(_client);
await commandHandler.InstallCommandsAsync();
//==============================================================================//
//Register DI//
var builder = new ContainerBuilder();
//Load all commands
var commands = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);
foreach (var command in commands)
{
	MethodInfo method = typeof(Autofac.RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
	method = method.MakeGenericMethod(command);
	method.Invoke(builder, new object[1] { builder });
}
//Load all configs
var settings = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("appsettings.json"));
builder.RegisterInstance<IConfiguration>(settings);
var items = JsonConvert.DeserializeObject<Items>(File.ReadAllText("items.json"));
builder.RegisterInstance<Items>(items);
var enemy = JsonConvert.DeserializeObject<Enemies>(File.ReadAllText("enemy.json"));
builder.RegisterInstance<Enemies>(enemy);
var map = JsonConvert.DeserializeObject<Maps>(File.ReadAllText("map.json"));
builder.RegisterInstance<Maps>(map);
//Load all Services
builder.RegisterType<PlayerService>().As<IPlayerService>();
builder.RegisterType<MapService>().As<IMapService>();
//todo: add all json into DI


//=============================================================================//
//Start Bot//
Data.Instance.Container = builder.Build();
var token = Data.Instance.Container.Resolve<IConfiguration>().Token;

await _client.LoginAsync(TokenType.Bot, token);
await _client.StartAsync();

await Task.Delay(-1);

Task Log(LogMessage msg)
{
	Console.WriteLine(msg.ToString());
	return Task.CompletedTask;
}