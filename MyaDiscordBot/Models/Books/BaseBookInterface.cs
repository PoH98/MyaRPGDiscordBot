using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models.Books
{
    public class Book
    {
        public virtual string Name { get; set; }
        public virtual BookType BType { get; set; }
        public int Amount { get; set; }
        
    }

    public enum BookType
    {
        Atk,
        Def,
        HP
    }
}
