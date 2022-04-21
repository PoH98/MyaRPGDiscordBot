using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.ButtonEvent
{
    public class SkillPointsHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        public SkillPointsHandler(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            if (command.StartsWith("skill-"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id, message.User.Username);
            if (player.SkillPoint < 1)
            {
                await message.RespondAsync("你無存在可以用的點數！請升級更多後先返來啦！（每5級會獲得一點數哦！）", ephemeral: true);
                return;
            }
            switch (message.Data.CustomId.Replace("skill-", ""))
            {
                case "atk":
                    player.Atk++;
                    break;
                case "def":
                    player.Def++;
                    break;
                case "hp":
                    player.HP++;
                    player.CurrentHP++;
                    break;
            }
            player.SkillPoint--;
            await message.RespondAsync("數值已增加！", ephemeral: true);
            playerService.SavePlayer(player);
        }
    }
}
