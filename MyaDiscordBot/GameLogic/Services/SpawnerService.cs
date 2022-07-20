using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface ISpawnerService
    {
        Enemy Spawn(Element mapType, int lv);
        Enemy SpawnBoss(int lv);
    }
    public class SpawnerService : ISpawnerService
    {
        private readonly Enemies _enemy;
        public SpawnerService(Enemies enemies)
        {
            _enemy = enemies;
        }
        public Enemy Spawn(Element mapType, int lv)
        {
            Random rnd = new Random();
            var probability = rnd.NextDouble();
            if (probability < 0.01)
            {
                return SpawnBoss(lv);
            }
            if (probability < 0.3)
            {
                //no spawn
                return null;
            }
            var rank = lv / 10;
            if (rank >= 10)
            {
                //player 100lv, infinite loop now
                var loopCount = (rank / 10) + 1;
                rank = rank % 10;
                var enemies = _enemy.Where(x => x.Element == mapType && x.Stage == rank && !x.IsBoss).ToList();
                var enemySelected = rnd.Next(enemies.Count());
                var enemy = (Enemy)enemies[enemySelected].Clone();
                enemy.Def *= loopCount * 8;
                enemy.Atk *= loopCount * 8;
                enemy.HP *= loopCount * 10;
                enemy.Stage = loopCount * 10 + rank;
                return enemy;
            }
            else
            {
                var enemies = _enemy.Where(x => x.Element == mapType && x.Stage == rank && !x.IsBoss).ToList();
                var enemySelected = rnd.Next(enemies.Count());
                return (Enemy)enemies[enemySelected].Clone();
            }
        }

        public Enemy SpawnBoss(int lv)
        {
            Random rnd = new Random();
            var stage = lv / 10;
            var bosses = _enemy.Where(x => x.IsBoss && x.Stage == stage).ToArray();
            if (bosses.Length < 1)
            {
                if (stage < 1)
                {
                    stage = 1;
                }
#if DEBUG
                //no boss, lets create one default boss
                return new Enemy
                {
                    Atk = 999,
                    Def = 999,
                    HP = 1000 * stage,
                    Element = Element.God,
                    IsBoss = true,
                    Stage = stage,
                    Name = "虛無之鬼"
                };
#else
                return new Enemy
                {
                    Atk = 20 * stage,
                    Def = 0,
                    HP = 100 * stage,
                    Element = Element.God,
                    IsBoss= true,
                    Stage = stage,
                    Name = "虛無之鬼"
                };
#endif
            }
            return (Enemy)bosses[rnd.Next(bosses.Count())].Clone();
        }
    }
}
