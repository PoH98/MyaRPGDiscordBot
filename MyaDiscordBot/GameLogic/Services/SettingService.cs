﻿using Discord.WebSocket;
using LiteDB;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface ISettingService
    {
        bool CorrectChannel(SocketCommandBase interaction, out ulong correctChannel);
        void SetChannel(SocketSlashCommand command);
        ServerSettings GetSettings(ulong serverId);
        void SaveSettings(ulong serverId, ServerSettings settings);
    }
    public class SettingService : ISettingService
    {
        private readonly IConfiguration configuration;
        public SettingService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public bool CorrectChannel(SocketCommandBase interaction, out ulong correctChannel)
        {
            using (var db = new LiteDatabase("Filename=save\\" + (interaction.Channel as SocketGuildChannel).Guild.Id + ".db;connection=shared"))
            {
                var guild = db.GetCollection<ServerSettings>("settings");
                if (!guild.Exists(x => x.GuildId == (interaction.Channel as SocketGuildChannel).Guild.Id))
                {
                    //not set
                    correctChannel = (interaction.Channel as SocketGuildChannel).Guild.DefaultChannel.Id;
                    return true;
                }
                else
                {
                    var setting = guild.FindOne(x => x.GuildId == (interaction.Channel as SocketGuildChannel).Guild.Id);
                    correctChannel = setting.ChannelId;
                    if ((interaction.Channel as SocketGuildChannel).Id == setting.ChannelId)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public ServerSettings GetSettings(ulong serverId)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var guild = db.GetCollection<ServerSettings>("settings");
                return guild.FindOne(x => x.GuildId == serverId);
            }
        }

        public void SaveSettings(ulong serverId, ServerSettings settings)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var guild = db.GetCollection<ServerSettings>("settings");
                guild.Update(settings);
            }
        }

        public void SetChannel(SocketSlashCommand command)
        {
            if (configuration.AdminId != command.User.Id)
            {
                throw new ArgumentException("Invalid permission");
            }
            using (var db = new LiteDatabase("Filename=save\\" + (command.Channel as SocketGuildChannel).Guild.Id + ".db;connection=shared"))
            {
                var guild = db.GetCollection<ServerSettings>("settings");
                if (!guild.Exists(x => x.GuildId == (command.Channel as SocketGuildChannel).Guild.Id))
                {
                    guild.Insert(new ServerSettings
                    {
                        ChannelId = (command.Channel as SocketGuildChannel).Id,
                        GuildId = (command.Channel as SocketGuildChannel).Guild.Id
                    });
                }
                else
                {
                    var set = guild.FindOne(x => x.GuildId == (command.Channel as SocketGuildChannel).Guild.Id);
                    set.ChannelId = (command.Channel as SocketGuildChannel).Id;
                    guild.Update(set);
                }
            }
        }
    }
}
