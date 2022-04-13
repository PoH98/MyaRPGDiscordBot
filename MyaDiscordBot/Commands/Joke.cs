using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Joke : ICommand
    {
        public string Name => "joke";

        public string Description => "Ask Mya to say a joke";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            if (!File.Exists("joke.txt"))
            {
                await command.RespondAsync("米亞搖搖頭表示佢未睇過任何搞笑野，需要大家收集！可以使用```/addjoke```功能添加新joke哦！");
                return;
            }
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                await command.RespondAsync("zzZZZ (米亞已經訓著，無法回復你哦！)", ephemeral: true);
                return;
            }
            var jokes = await File.ReadAllLinesAsync("joke.txt");
            Random rnd = new Random();
            var index = rnd.Next(jokes.Length);
            while (Data.Instance.LastRnd == index && jokes.Length > 1)
            {
                //no
                index = rnd.Next(jokes.Length);
            }
            Data.Instance.LastRnd = index;
            await command.RespondAsync(jokes[index] + "\nFunny~");
        }
    }
}
