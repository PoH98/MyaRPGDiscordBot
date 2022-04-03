using LiteDB;
using MyaDiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IMapService
    {
        Map GetCurrentMap(ulong serverId);
        void NextStage(int serverId);
        Enemy SpawnEnemy(Coordinate coordinate, Map map);
    }
    internal class MapService : IMapService
    {
        private readonly Maps maps;
        public MapService(Maps maps)
        {
            this.maps= maps;
        }
        public Map GetCurrentMap(ulong serverId)
        {
            if (!File.Exists(serverId + ".json"))
            {
                File.WriteAllText(serverId + ".json", JsonConvert.SerializeObject(new Save
                {
                    CurrentStage = 1,
                    CurrentBoss = null,
                    EnemyLeft = 15000
                }));
            }
            var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(serverId + ".json"));
            return maps[save.CurrentStage - 1];
        }

        public void NextStage(int serverId)
        {
            var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(serverId + ".json"));
            save.CurrentStage++;
            if(save.CurrentStage >= maps.Count)
            {
                save.CurrentStage = 1;
            }
            File.WriteAllText(serverId + ".json", JsonConvert.SerializeObject(save));
        }

        public Enemy SpawnEnemy(Coordinate coordinate, Map map)
        {
            Enemy result = null;
            var tiles = GetTiles(coordinate, map);
            if(tiles.Count() == 1)
            {
                //just one gene
                switch (tiles.First().Tile)
                {
                    case MapItem.Wall:
                        throw new ArgumentException("Hacker found! How the fuck you get into between walls");
                }
            }
            else
            {
                //need think more on how to calculate the tile weight
            }
            return result;
        }

        private IEnumerable<GroupedTiles> GetTiles(Coordinate coordinate, Map map)
        {
            //check 3x3 region if exist other than land, if so calculate spawn specific element enemies
            var nineX = new List<MapItem>();
            //[x][ ][ ]
            //[ ][0][ ]
            //[ ][ ][ ]
            nineX.Add(map.MapData[coordinate.X - 1][coordinate.Y + 1]);
            //[ ][x][ ]
            //[ ][0][ ]
            //[ ][ ][ ]
            nineX.Add(map.MapData[coordinate.X][coordinate.Y + 1]);
            //[ ][ ][x]
            //[ ][0][ ]
            //[ ][ ][ ]
            nineX.Add(map.MapData[coordinate.X + 1][coordinate.Y + 1]);
            //[ ][ ][ ]
            //[x][0][ ]
            //[ ][ ][ ]
            nineX.Add(map.MapData[coordinate.X - 1][coordinate.Y]);
            //[ ][ ][ ]
            //[ ][0][x]
            //[ ][ ][ ]
            nineX.Add(map.MapData[coordinate.X + 1][coordinate.Y]);
            //[ ][ ][ ]
            //[ ][0][ ]
            //[x][ ][ ]
            nineX.Add(map.MapData[coordinate.X - 1][coordinate.Y - 1]);
            //[ ][ ][ ]
            //[ ][0][ ]
            //[ ][x][ ]
            nineX.Add(map.MapData[coordinate.X][coordinate.Y - 1]);
            //[ ][ ][ ]
            //[ ][0][ ]
            //[ ][ ][x]
            nineX.Add(map.MapData[coordinate.X + 1][coordinate.Y - 1]);
            //group all tiles and calculate counts
            return nineX.GroupBy(x => x).Select(group => new GroupedTiles { Tile = group.Key, Count = group.Count() }).OrderBy(x => x.Count);
        }
    }
}
