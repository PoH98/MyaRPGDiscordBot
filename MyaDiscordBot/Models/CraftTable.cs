namespace MyaDiscordBot.Models
{
    public class CraftTableList : List<CraftTable>
    {

    }
    public class CraftTable
    {
        public List<RequiredResource> Resources { get; set; }
        public Guid Item { get; set; }
    }

    public class RequiredResource
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
    }
}
