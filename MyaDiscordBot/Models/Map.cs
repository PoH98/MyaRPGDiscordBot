using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class Maps : List<Map>
    {

    }
    public class Map
    {
        public List<List<MapItem>> MapData { get; set; }
        public Coordinate SpawnCoordinate
        {
            get
            {
                var y = MapData.IndexOf(MapData.Where(x => x.Any(y => y == MapItem.PlayerSpawn)).First());
                var x = MapData[y].IndexOf(MapItem.PlayerSpawn);
                return new Coordinate
                {
                    X = x,
                    Y = y
                };
            }
        }
    }

    public enum MapItem
    {
        Wall = 0,
        Land = 1,
        Water = 2,
        PlayerSpawn = 3,
        Lava = 4
    }

    public class GroupedTiles
    {
        public MapItem Tile { get; set; }
        public int Count { get; set; }
    }
}
