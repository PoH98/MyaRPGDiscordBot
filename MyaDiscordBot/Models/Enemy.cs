using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class Enemies : List<Enemy>
    {

    }

    public class Enemy
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public Element Element { get; set; }
        public decimal ItemDropRate { get; set; }
    }
}
