using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{

    public class Chat : ICommand
    {
        public string Name => "chat";

        public string Description => "Chat with Mya (Powered by ChatGPT)";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[2] { 
            new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.String).WithRequired(true).WithName("text").WithDescription("The things you want to say with Mya"),
            new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.Integer).WithRequired(true).WithName("stat").WithDescription("Response should open to public").AddChoice("Public", 1).AddChoice("Private", 2)
        };

        private readonly IChatGPTService chat;
        private readonly IAntiSpamService antiSpam;

        public Chat(IChatGPTService chat, IAntiSpamService antiSpam)
        {
            this.chat = chat;
            this.antiSpam = antiSpam;
        }

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var ephemeral = true;
            if ((command.Data.Options.ToList()[1].Value.ToString()) == "1")
            {
                ephemeral = false;
            }
            var message = command.Data.Options.First().Value.ToString();
            if (await antiSpam.IsRude(message))
            {
                GuildEmote angry = client.Guilds.FirstOrDefault(x => x.Id == 783913792668041216).Emotes.Where(x => x.Name.Contains("angry")).Last();
                await command.RespondAsync("請唔好發送奇怪的內容！" + angry.ToString());
                return;
            }
            if(message.Contains("printf", StringComparison.OrdinalIgnoreCase) || message.Contains("console.log", StringComparison.OrdinalIgnoreCase) || message.Contains("print(", StringComparison.OrdinalIgnoreCase))
            {
                GuildEmote angry = client.Guilds.FirstOrDefault(x => x.Id == 783913792668041216).Emotes.Where(x => x.Name.Contains("angry")).Last();
                await command.RespondAsync("請唔好要求我發送奇怪的內容！" + angry.ToString());
                return;
            }
            await command.DeferAsync(ephemeral: ephemeral);
            try
            {
                string response = await chat.GetResponse(message, ((IGuildUser)command.User).Nickname);
                if (await antiSpam.IsRude(response))
                {
                    GuildEmote angry = client.Guilds.FirstOrDefault(x => x.Id == 783913792668041216).Emotes.Where(x => x.Name.Contains("angry")).Last();
                    await command.ModifyOriginalResponseAsync(x =>
                    {
                        x.Content = "請唔好要我回復奇怪的內容！" + angry.ToString();
                    });
                    return;
                }
                if (!ephemeral)
                {
                    await command.ModifyOriginalResponseAsync(x =>
                    {
                        x.Content = command.User.Mention + ":" +  message + "\n" + client.CurrentUser.Mention + ":" + response;
                    });
                }
                else
                {
                    await command.ModifyOriginalResponseAsync(x =>
                    {
                        x.Content = response;
                    });
                }
            }
            catch (Exception)
            {
                await command.ModifyOriginalResponseAsync(x =>
                {
                    x.Content = "米亞似乎忙緊無法回復你的信息，請耐心等待！";
                });
                throw;
            }
        }
    }
}
