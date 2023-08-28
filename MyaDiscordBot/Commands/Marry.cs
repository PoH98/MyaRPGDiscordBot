using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    internal class Marry : ICommand
    {
        private readonly IPlayerService playerService;
        public Marry(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "marry";

        public string Description => "marry with other players!";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[] { 
            new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.User).WithRequired(true).WithName("name").WithDescription("The user you wants to marry with (?)") };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            Player player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            var user = ((SocketGuildUser)command.Data.Options.First().Value);
            var targetUser = playerService.LoadPlayer(user.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            if(user.Id == command.User.Id)
            {
                await command.RespondAsync("CLS, " + command.User.Mention + "竟然想自己同自己結婚，有咩心裡變態？？冇女就自己扮女同自己結婚的心態好應該睇下米亞醫院的心理醫生啊！");
                return;
            }
            if (player.MarriedUser == 0 && targetUser.MarriedUser == 0)
            {
                targetUser.MarriedUser = command.User.Id;
                targetUser.MarriedTime = DateTime.Now;
                playerService.SavePlayer(targetUser);
                player.MarriedUser = user.Id;
                player.MarriedTime = DateTime.Now;
                playerService.SavePlayer(player);
                await command.RespondAsync("恭喜嗮" + command.User.Mention + "與" + user.Mention + "結為夫妻！");
            }
            else if(targetUser.MarriedUser == 0)
            {
                targetUser.MarriedUser = command.User.Id;
                targetUser.MarriedTime = DateTime.Now;
                playerService.SavePlayer(targetUser);
                var oldId = player.MarriedUser;
                var oldPlayer = playerService.LoadPlayer(oldId, (command.Channel as SocketGuildChannel).Guild.Id);
                oldPlayer.MarriedUser = 0;
                oldPlayer.MarriedTime = DateTime.Now;
                playerService.SavePlayer(oldPlayer);
                var oldUser = await client.GetUserAsync(oldId);
                var oldTime = player.MarriedTime;
                player.MarriedUser = user.Id;
                player.MarriedTime = DateTime.Now;
                playerService.SavePlayer(player);
                await command.RespondAsync("恭喜嗮" + command.User.Mention + "與" + user.Mention + "結為夫妻，並且拋棄左與佢結婚左" + Math.Round((DateTime.Now - oldTime).TotalDays).ToString() + "日的" + oldUser.Mention + "！真係好過分啊！");
            }
            else if(targetUser.MarriedUser == command.User.Id)
            {
                targetUser.MarriedUser = 0;
                player.MarriedUser = 0;
                playerService.SavePlayer(targetUser);
                playerService.SavePlayer(player);
                var marriedTime = (DateTime.Now - player.MarriedTime).TotalDays;
                await command.RespondAsync("估唔到" + command.User.Mention + "咁狠心，拋棄左同自己結婚左" + marriedTime + "日的" + user.Mention + "！兩人的婚姻就此結束！");
            }
            else
            {
                await command.RespondAsync("估唔到" + command.User.Mention + "竟然想搶走" + user.Mention + "結為夫妻！真係好過分啊！米亞忍唔住打左" + command.User.Mention + "一巴掌！");
                player.CurrentHP = 1;
                playerService.SavePlayer(player);
            }
        }
    }
}
