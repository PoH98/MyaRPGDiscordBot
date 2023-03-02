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
