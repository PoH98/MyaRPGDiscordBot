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
            if (probability < 0.3)
            {
                //no spawn
                return null;
            }
            var enemies = _enemy.Where(x => x.Element == mapType && x.Stage == ((lv / 10) < 1 ? 1 : (lv / 10)) && !x.IsBoss).ToList();
            var enemySelected = rnd.Next(enemies.Count());
            return (Enemy)enemies[enemySelected].Clone();
        }

        public Enemy SpawnBoss(int lv)
        {
            Random rnd = new Random();
            var bosses = _enemy.Where(x => x.IsBoss && x.Stage == lv / 10).ToArray();
            if (bosses.Length < 1)
            {
#if DEBUG
                //no boss, lets create one default boss
                return new Enemy
                {
                    Atk = 100 * lv / 10,
                    Def = 0,
                    HP = 1 * lv / 10,
                    Element = Element.God,
                    IsBoss = true,
                    Stage = lv / 10,
                    Name = "虛無之鬼"
                };
#else
                return new Enemy
                {
                    Atk = 100 * stage,
                    Def = 0,
                    HP = 1000 * stage,
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
