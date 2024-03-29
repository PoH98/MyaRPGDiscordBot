﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;

namespace MyaDiscordBot.Commands
{
    //Disabled first since we havent able to add blacklist yet
    public class Blacklist : ICommand
    {
        public string Name => "blacklist";

        public string Description => "Blacklist user from the server, also from other servers with same database used";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[2] { new SlashCommandOptionBuilder().WithName("user").WithDescription("User to be blacklisted").WithType(ApplicationCommandOptionType.User).WithRequired(true), new SlashCommandOptionBuilder().WithName("reason").WithDescription("Blacklist reason").WithRequired(true).WithType(ApplicationCommandOptionType.Integer).AddChoice("Ads", 1).AddChoice("Scam", 2) };

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            SocketGuildUser user = command.User as SocketGuildUser;
            if (((SocketGuildUser)command.Data.Options.First().Value).Id == client.CurrentUser.Id)
            {
                //the fuck you report??
                GuildEmote angry = client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("angry")).Last();
                return command.RespondAsync("我XXXXXXX？！竟然Blacklist我？！食屎啦你！！" + angry.ToString());
            }
            if (command.User.Id == 294835963442757632 || (!command.User.IsBot && (user.GuildPermissions.BanMembers || user.GuildPermissions.KickMembers || user.GuildPermissions.Administrator || user.GuildPermissions.ManageGuild)))
            {
                Console.WriteLine("Confirming blacklist " + ((SocketGuildUser)command.Data.Options.First().Value).DisplayName);
                ComponentBuilder cb = new();
                _ = cb.WithButton("確認", "ban-" + ((SocketGuildUser)command.Data.Options.First().Value).Id + "-" + command.Data.Options.Last().Value, ButtonStyle.Danger);
                return command.RespondAsync("是否真要Ban" + ((SocketGuildUser)command.Data.Options.First().Value).Mention + "? 此舉動會令佢以後的Discord生活會極度淒慘並且無法取消此舉動！", components: cb.Build(), ephemeral: true);
            }
            return command.RespondAsync("咩啊，" + ((SocketGuildUser)command.Data.Options.First().Value).Mention + "對你做左咩你要Blacklist佢啊？可惜你無權限囖白癡！我要公告天下你亂Blacklist人" + command.User.Mention + "！！");
        }
    }
}
