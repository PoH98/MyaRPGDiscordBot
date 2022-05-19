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
            using (var scope = Data.Instance.Container.BeginLifetimeScope())
            {
                var bossService = scope.Resolve<IBossService>();
                var client = scope.Resolve<DiscordSocketClient>();
                var settingService = scope.Resolve<ISettingService>();
                foreach (var file in Directory.GetFiles("save", "*.db"))
                {
                    //get joined servers
                    try
                    {
                        var guild = client.GetGuild(Convert.ToUInt64(file.Remove(0, file.LastIndexOf("\\") + 1).Replace(".db", "")));
                        bossService.RemoveExpired(guild.Id);
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
                            var setting = settingService.GetSettings(guild.Id);
                            if (setting != null)
                            {
                                await guild.GetTextChannel(setting.ChannelId).SendMessageAsync("米講粗口亞再次出現啦！請玩家們記得討伐獲得獎勵！");
                            }
                            else
                            {
                                try
                                {
                                    await guild.GetTextChannel(guild.DefaultChannel.Id).SendMessageAsync("米講粗口亞再次出現啦！請玩家們記得討伐獲得獎勵！");
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
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var col = db.GetCollection<BossSpawned>("boss");
                if (enemy.Name != "米講粗口亞")
                {
                    col.Insert(new BossSpawned
                    {
                        Enemy = enemy,
                        ExpiredTime = DateTime.Now.AddDays(1),
                        GuildId = serverId
                    });
                }
                else
                {
                    col.Insert(new BossSpawned
                    {
                        Enemy = enemy,
                        ExpiredTime = DateTime.Now.AddDays(5),
                        GuildId = serverId
                    });
                }
            }
        }

        public void DefeatedEnemy(ulong serverId, BossSpawned enemy)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var col = db.GetCollection<BossSpawned>("boss");
                col.Delete(enemy.Id);
            }
        }

        public IEnumerable<BossSpawned> GetEnemy(ulong serverId)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var col = db.GetCollection<BossSpawned>("boss");
                var list = col.Find(x => x.GuildId == serverId).ToList();
                if (list.Count > 0)
                {
                    return list.Where(x => DateTime.Compare(x.ExpiredTime, DateTime.Now) > 0);
                }
                return list;
            }
        }

        public void RemoveExpired(ulong serverId)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var col = db.GetCollection<BossSpawned>("boss");
                var data = col.Find(x => x.GuildId == serverId);
                if (data == null)
                {
                    return;
                }
                foreach (var i in data.Where(x => DateTime.Compare(x.ExpiredTime, DateTime.Now) < 0))
                {
                    try
                    {
                        col.Delete(i.Id);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void UpdateEnemy(ulong serverId, BossSpawned enemy)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var col = db.GetCollection<BossSpawned>("boss");
                col.Update(enemy);
            }
        }
    }
}
