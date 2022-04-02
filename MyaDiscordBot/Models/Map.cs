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
    }

    public enum MapItem
    {
        Wall = 0,
        Land = 1,
        Water = 2,
        PlayerSpawn = 3,
        Lava = 4
    }
}
