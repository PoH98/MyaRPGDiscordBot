using Autofac;
using Discord.WebSocket;
using LiteDB;
using MyaDiscordBot.Models;
using Quartz;

namespace MyaDiscordBot.GameLogic.Services
{
    //not handjob ok?
    public class BossJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Boss scheduller running");
            using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
            IBossService bossService = scope.Resolve<IBossService>();
            DiscordSocketClient client = scope.Resolve<DiscordSocketClient>();
            ISettingService settingService = scope.Resolve<ISettingService>();
            foreach (string file in Directory.GetFiles("save", "*.db"))
            {
                //get joined servers
                try
                {
                    SocketGuild guild = client.GetGuild(Convert.ToUInt64(file.Remove(0, file.LastIndexOf("\\") + 1).Replace(".db", "")));
                    bossService.RemoveExpired(guild.Id);
                    try
                    {
                        if (DateTime.Now.Day == 1 && DateTime.Now.Month == 1 && DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
                        {
                            ServerSettings setting = settingService.GetSettings(guild.Id);
                            Discord.GuildEmote yeah = client.Guilds.SelectMany(x => x.Emotes).Where(x => x.Name.Contains("yeah")).Last();
                            if (setting != null)
                            {
                                _ = await guild.GetTextChannel(setting.ChannelId).SendMessageAsync(DateTime.Now.Year + "新年快樂啊大家！" + yeah.ToString());
                            }
                            else
                            {
                                try
                                {
                                    _ = await guild.GetTextChannel(guild.DefaultChannel.Id).SendMessageAsync(DateTime.Now.Year + "新年快樂啊大家！" + yeah.ToString());
                                }
                                catch
                                {
                                    //any error will ignored
                                }
                            }
                        }
                    }
                    catch
                    {

                    }

                    if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0 && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {

                        bossService.AddBoss(guild.Id, new Enemy
                        {
                            Name = "米講粗口亞",
                            Atk = 999,
                            Def = 0,
                            HP = 99999,
                            Element = Element.God,
                            IsBoss = true,
                            Stage = 999
                        });
                        ServerSettings setting = settingService.GetSettings(guild.Id);
                        if (setting != null)
                        {
                            _ = await guild.GetTextChannel(setting.ChannelId).SendMessageAsync("米講粗口亞再次出現啦！請玩家們記得討伐獲得獎勵！");
                        }
                        else
                        {
                            try
                            {
                                _ = await guild.GetTextChannel(guild.DefaultChannel.Id).SendMessageAsync("米講粗口亞再次出現啦！請玩家們記得討伐獲得獎勵！");
                            }
                            catch
                            {
                                //any error will ignored
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            //offline reward
            IPlayerService playerService = scope.Resolve<IPlayerService>();
            foreach (string file in Directory.GetFiles("save", "*.db"))
            {
                //get joined servers
                try
                {
                    SocketGuild guild = client.GetGuild(Convert.ToUInt64(file.Remove(0, file.LastIndexOf("\\") + 1).Replace(".db", "")));
                    if (guild == null)
                    {
                        //guild is not exist anymore, bot is kicked
                        File.Delete(file);
                        continue;
                    }
                    List<Player> players = playerService.GetPlayers(guild.Id);
                    foreach (Player player in players)
                    {
                        try
                        {
                            //get last command time
                            if (player.LastCommand == DateTime.MinValue)
                            {
                                player.LastCommand = DateTime.Now;
                            }
                            int awaitTime = (DateTime.Now - player.LastCommand).Hours;
                            if (awaitTime < 1)
                            {
                                continue;
                            }
                            int coinGet = awaitTime * 10 * (player.Lv / 10);
                            Random random = new();
                            int coinLost = (int)Math.Round((random.NextDouble() * ((awaitTime * 3.5) - (awaitTime * 1.5))) + (awaitTime * 1.5));
                            coinGet -= coinLost;
                            if (coinGet <= 0)
                            {
                                coinGet = 1;
                            }
                            player.Coin += coinGet;
                            player.Exp += (int)Math.Round(coinGet * 0.2);
                            player.LastCommand = DateTime.Now;
                            if(player.Exp >= 100000)
                            {
                                player.Exp = 100000;
                            }
                            playerService.SavePlayer(player);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }
    }

    public interface IBossService
    {
        void AddBoss(ulong serverId, Enemy enemy);
        IEnumerable<BossSpawned> GetEnemy(ulong serverId);
        void UpdateEnemy(ulong serverId, BossSpawned enemy);
        void RemoveExpired(ulong serverId);
        void DefeatedEnemy(ulong serverId, BossSpawned enemy);
    }

    public class BossService : IBossService
    {
        public void AddBoss(ulong serverId, Enemy enemy)
        {
            using LiteDatabase db = new("Filename=save\\" + serverId + ".db;connection=shared");
            ILiteCollection<BossSpawned> col = db.GetCollection<BossSpawned>("boss");
            _ = enemy.Name != "米講粗口亞"
                ? col.Insert(new BossSpawned
                {
                    Enemy = enemy,
                    ExpiredTime = DateTime.Now.AddDays(1),
                    GuildId = serverId
                })
                : col.Insert(new BossSpawned
                {
                    Enemy = enemy,
                    ExpiredTime = DateTime.Now.AddDays(5),
                    GuildId = serverId
                });
        }

        public void DefeatedEnemy(ulong serverId, BossSpawned enemy)
        {
            using LiteDatabase db = new("Filename=save\\" + serverId + ".db;connection=shared");
            ILiteCollection<BossSpawned> col = db.GetCollection<BossSpawned>("boss");
            _ = col.Delete(enemy.Id);
        }

        public IEnumerable<BossSpawned> GetEnemy(ulong serverId)
        {
            using LiteDatabase db = new("Filename=save\\" + serverId + ".db;connection=shared");
            ILiteCollection<BossSpawned> col = db.GetCollection<BossSpawned>("boss");
            List<BossSpawned> list = col.Find(x => x.GuildId == serverId).ToList();
            return list.Count > 0 ? list.Where(x => DateTime.Compare(x.ExpiredTime, DateTime.Now) > 0) : list;
        }

        public void RemoveExpired(ulong serverId)
        {
            using LiteDatabase db = new("Filename=save\\" + serverId + ".db;connection=shared");
            ILiteCollection<BossSpawned> col = db.GetCollection<BossSpawned>("boss");
            IEnumerable<BossSpawned> data = col.Find(x => x.GuildId == serverId);
            if (data == null)
            {
                return;
            }
            foreach (BossSpawned i in data.Where(x => DateTime.Compare(x.ExpiredTime, DateTime.Now) < 0))
            {
                try
                {
                    _ = col.Delete(i.Id);
                }
                catch
                {

                }
            }
        }

        public void UpdateEnemy(ulong serverId, BossSpawned enemy)
        {
            using LiteDatabase db = new("Filename=save\\" + serverId + ".db;connection=shared");
            ILiteCollection<BossSpawned> col = db.GetCollection<BossSpawned>("boss");
            _ = col.Update(enemy);
        }
    }
}
