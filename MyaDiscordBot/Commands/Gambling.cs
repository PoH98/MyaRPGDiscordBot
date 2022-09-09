using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Gambling : ICommand
    {
        private readonly IPlayerService playerService;
        public Gambling(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "gamble";

        public string Description => "Play some gambling";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1] { new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.Integer).AddChoice("21 Point", 1).WithDescription("Gambling Type").WithName("type").WithRequired(true) };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            if(DateTime.Compare(player.GamblingDelay, DateTime.Now) > 0)
            {
                await command.RespondAsync("米亞唔同你再賭啦！你個死賭鬼！", ephemeral: true);
                return;
            }
            switch (Convert.ToInt32(command.Data.Options.First().Value))
            {
                case 1:
                    var cards = new List<int> { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
                    if (player.Coin < 300)
                    {
                        //no money to play this
                        await command.RespondAsync("你個死窮鬼，死走啦，賭咩賭！至少150蚊先搵米亞啦！", ephemeral: true);
                        return;
                    }
                    Random rnd = new Random();
                    int pldrawCount = 0, medrawCount = 0;
                    List<int> plCards = new List<int>();
                    List<int> meCards = new List<int>();
                    do
                    {
                        var index = rnd.Next(cards.Count);
                        var card = cards[index];
                        if (plCards.Sum() <= 11 && card == 1)
                        {
                            card = 11;
                        }
                        if (plCards.Sum() > 21 && plCards.Any(x => x == 11))
                        {
                            var iv = plCards.IndexOf(plCards.First(x => x == 11));
                            plCards[iv] = 1;
                        }
                        plCards.Add(card);
                        cards.Remove(index);
                        pldrawCount++;
                    }
                    while (plCards.Sum() < 17 && pldrawCount < 5);
                    do
                    {
                        var index = rnd.Next(cards.Count);
                        var card = cards[index];
                        if (meCards.Sum() <= 11 && card == 1)
                        {
                            card = 11;
                        }
                        if (meCards.Sum() > 21 && meCards.Any(x => x == 11))
                        {
                            var iv = meCards.IndexOf(meCards.First(x => x == 11));
                            meCards[iv] = 1;
                        }
                        meCards.Add(card);
                        cards.Remove(index);
                        medrawCount++;
                    }
                    while (meCards.Sum() < 17 && medrawCount < 5);
                    var pl = plCards.Sum();
                    var me = meCards.Sum();
                    if (pl == 21 || me == 21)
                    {
                        if (pl == me)
                        {
                            //draw
                            player.Coin -= 25;
                            await command.RespondAsync("你同米亞打成平手，米亞唔服氣照樣從你身上搶走左25蚊買屎玩！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                        }
                        else if (medrawCount == 4 || pldrawCount == 4)
                        {
                            if (pl == 21)
                            {
                                //player draw 5 times and 21
                                player.Coin += 300;
                                await command.RespondAsync("恭喜你抽左5張卡而且剛剛好21點！成功獲得300蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                            else
                            {
                                //bot draw 5 times and 21
                                player.Coin -= 300;
                                await command.RespondAsync("恭喜米亞抽左5張卡而且剛剛好21點！你輸左300蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                        }
                        else
                        {
                            if (pl == 21)
                            {
                                //player get 21
                                player.Coin += 150;
                                await command.RespondAsync("你獲得21點！成功得到150蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                            else
                            {
                                //bot get 21
                                player.Coin -= 150;
                                await command.RespondAsync("米亞獲得21點！輸左150蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                        }
                    }
                    else if (medrawCount >= 4 || pldrawCount >= 4)
                    {
                        if (medrawCount >= 4 && pldrawCount >= 4)
                        {
                            if (pl < 21 && me < 21)
                            {
                                if (pl > me)
                                {
                                    //player draw 5 times but still not 21
                                    player.Coin += 100;
                                    await command.RespondAsync("恭喜你抽左5張卡都未爆21點！成功獲得100蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                                }
                                else if (pl < me)
                                {
                                    //player draw 5 times but still not 21
                                    player.Coin -= 100;
                                    await command.RespondAsync("恭喜米亞抽左5張卡都未爆21點！你輸左100蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                                }
                                else
                                {
                                    //nothing
                                    player.Coin -= 25;
                                    await command.RespondAsync("你同米亞打成平手，米亞唔服氣照樣從你身上搶走左25蚊買屎玩！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                                }
                            }
                            else
                            {
                                //lose
                                if (pl > 21 && me < 21)
                                {
                                    player.Coin -= 50;
                                    await command.RespondAsync("你的卡超過21點！你輸左50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                                }
                                else if (me > 21 && pl < 21)
                                {
                                    //bot more than 21
                                    player.Coin += 50;
                                    await command.RespondAsync("米亞的卡超過21點！你獲得50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                                }
                                else
                                {
                                    //nothing
                                    player.Coin -= 25;
                                    await command.RespondAsync("你同米亞打成平手，米亞唔服氣照樣從你身上搶走左25蚊買屎玩！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                                }
                            }
                        }
                        else if (medrawCount >= 4)
                        {
                            if (me < 21)
                            {
                                //bot draw 5 times but still not 21
                                player.Coin -= 100;
                                await command.RespondAsync("恭喜米亞抽左5張卡都未爆21點！你輸左100蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                            else if (pl < 21)
                            {
                                //bot more than 21
                                player.Coin += 50;
                                await command.RespondAsync("米亞的卡超過21點！你獲得50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                            else
                            {
                                //nothing
                                player.Coin -= 25;
                                await command.RespondAsync("你同米亞打成平手，米亞唔服氣照樣從你身上搶走左25蚊買屎玩！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                        }
                        else if (pldrawCount >= 4)
                        {
                            if (pl < 21)
                            {
                                //player draw 5 times but still not 21
                                player.Coin += 100;
                                await command.RespondAsync("恭喜你抽左5張卡都未爆21點！成功獲得100蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                            else if (me < 21)
                            {
                                //bot more than 21
                                player.Coin -= 50;
                                await command.RespondAsync("你的卡超過21點！輸左50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                            else
                            {
                                //nothing
                                player.Coin -= 25;
                                await command.RespondAsync("你同米亞打成平手，米亞唔服氣照樣從你身上搶走左25蚊買屎玩！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                            }
                        }
                    }
                    else if (me < 21 && pl < 21)
                    {
                        if (pl > me)
                        {
                            //player win
                            player.Coin += 50;
                            await command.RespondAsync("你贏左啦！獲得50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                        }
                        else if (me > pl)
                        {
                            //player lose
                            player.Coin -= 50;
                            await command.RespondAsync("米亞贏左啦！輸左50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                        }
                        else
                        {
                            //nothing
                            player.Coin -= 25;
                            await command.RespondAsync("你同米亞打成平手，米亞唔服氣照樣從你身上搶走左25蚊買屎玩！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                        }
                    }
                    else
                    {
                        if (me > 21 && pl < 21)
                        {
                            //bot more than 21
                            player.Coin += 50;
                            await command.RespondAsync("米亞的卡超過21點！你獲得50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                        }
                        else if (pl > 21 && me < 21)
                        {
                            //player more than 21
                            player.Coin -= 50;
                            await command.RespondAsync("你的卡超過21點！你輸左50蚊！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                        }
                        else
                        {
                            //nothing
                            player.Coin -= 25;
                            await command.RespondAsync("你同米亞打成平手，米亞唔服氣照樣從你身上搶走左25蚊買屎玩！\n你的點數：" + pl + "\n米亞點數：" + me, ephemeral: true);
                        }
                    }
                    player.GamblingTimes++;
                    if (player.GamblingTimes > 10)
                    {
                        player.GamblingDelay = DateTime.Now.AddHours(1);
                        player.GamblingTimes = 0;
                    }
                    playerService.SavePlayer(player);
                    break;
            }
        }
    }
}
