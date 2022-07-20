using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class Pet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public int Price { get; set; }
        public Ability Ability { get; set; }
        public float AbilityRate { get; set; }
    }
}
