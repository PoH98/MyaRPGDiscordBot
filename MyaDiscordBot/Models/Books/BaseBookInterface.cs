using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models.Books
{
    public interface BaseBookInterface
    {
        string Name { get; }
        BookType BType { get; }
        
    }

    public enum BookType
    {
        Atk,
        Def,
        HP
    }

    public class AttackBook : BaseBookInterface
    {
        public string Name => "AV書碎片";

        public BookType BType => BookType.Atk;
    }

    public class DefenceBook : BaseBookInterface
    {
        public string Name => "D Cup書碎片";

        public BookType BType => BookType.Def;
    }

    public class HPBook : BaseBookInterface
    {
        public string Name => "H漫書碎片";

        public BookType BType => BookType.HP;
    }
}
