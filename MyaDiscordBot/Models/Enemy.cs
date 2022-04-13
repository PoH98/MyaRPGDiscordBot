using Newtonsoft.Json;

namespace MyaDiscordBot.Models
{
    public class Enemies : List<Enemy>
    {

    }

    public class Enemy : ICloneable
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public Element Element { get; set; }
        public decimal ItemDropRate { get; set; }
        public int Stage { get; set; } = 1;
        public int[] DropRank { get; set; } = new int[1] { 1 };
        public bool IsBoss { get; set; } = false;
        public object Clone()
        {
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<Enemy>(JsonConvert.SerializeObject(this), deserializeSettings);
        }
    }
}
