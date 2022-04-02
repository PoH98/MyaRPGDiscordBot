using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class Player
    {
        //Atk, Def會從Bag入邊計算，所以唔需要
        public int Id { get; set; }
        public int Coin { get; set; }
        public int HP { get; set; }
        public List<string> Title { get; set; }
        public List<ItemEquip> Bag { get; set; }
        public Coordinate Coordinate { get; set; }
    }

    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
