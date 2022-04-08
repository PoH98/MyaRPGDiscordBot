using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface ISpawnerService
    {
        Enemy Spawn(int stage, params MapItem[] tile);
        Enemy SpawnBoss(int stage);
    }
    public class SpawnerService : ISpawnerService
    {
        private readonly Enemies _enemy;
        public SpawnerService(Enemies enemies)
        {
            _enemy = enemies;
        }
        public Enemy Spawn(int stage, params MapItem[] tile)
        {
            if (tile.Length == 0)
            {
                return null;
            }
            Random rnd = new Random();
            var selectedGene = tile.Length > 1 ? tile[rnd.Next(tile.Length - 1)] : tile[0];
            var probability = rnd.NextDouble();
            if (probability < 0.3)
            {
                //no spawn
                return null;
            }
            switch (selectedGene)
            {
                case MapItem.Wall:
                    var enemies = _enemy.Where(x => x.Element == Element.Earth && x.Stage == stage && !x.IsBoss).ToList();
                    if (enemies.Count == 0)
                    {
                        return null;
                    }
                    var enemySelected = rnd.Next(enemies.Count() - 1);
                    return (Enemy)enemies[enemySelected].Clone();
                case MapItem.Water:
                    enemies = _enemy.Where(x => x.Element == Element.Water && x.Stage == stage && !x.IsBoss).ToList();
                    if (enemies.Count == 0)
                    {
                        return null;
                    }
                    enemySelected = rnd.Next(enemies.Count() - 1);
                    return (Enemy)enemies[enemySelected].Clone();
                case MapItem.Lava:
                    enemies = _enemy.Where(x => x.Element == Element.Fire && x.Stage == stage && !x.IsBoss).ToList();
                    if (enemies.Count == 0)
                    {
                        return null;
                    }
                    enemySelected = rnd.Next(enemies.Count() - 1);
                    return (Enemy)enemies[enemySelected].Clone();
                case MapItem.Land:
                    enemies = _enemy.Where(x => x.Element == Element.Wind && x.Stage == stage && !x.IsBoss).ToList();
                    if (enemies.Count == 0)
                    {
                        return null;
                    }
                    enemySelected = rnd.Next(enemies.Count() - 1);
                    return (Enemy)enemies[enemySelected].Clone();
            }
            return null;
        }

        public Enemy SpawnBoss(int stage)
        {
            Random rnd = new Random();
            var bosses = _enemy.Where(x => x.IsBoss && x.Stage == stage).ToArray();
            if(bosses.Length < 1)
            {
#if DEBUG
                //no boss, lets create one default boss
                return new Enemy
                {
                    Atk = 100 * stage,
                    Def = 0,
                    HP = 1 * stage,
                    Element = Element.God,
                    IsBoss= true,
                    Stage = stage,
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
            return (Enemy)bosses[rnd.Next(bosses.Count() - 1)].Clone();
        }
    }
}
