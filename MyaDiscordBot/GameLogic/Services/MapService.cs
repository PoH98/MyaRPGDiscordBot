using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IMapService
    {
        Enemy SpawnEnemy(Element map, int lv);
        Enemy SpawnBoss(int lv);
    }
    internal class MapService : IMapService
    {
        private readonly ISpawnerService spawnerService;
        public MapService(ISpawnerService spawnerService)
        {
            this.spawnerService = spawnerService;
        }

        public Enemy SpawnEnemy(Element map, int lv)
        {
            return spawnerService.Spawn(map, lv);
        }

        public Enemy SpawnBoss(int lv)
        {
            return spawnerService.SpawnBoss(lv);
        }
    }
}
