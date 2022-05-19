using LiteDB;
using MyaDiscordBot.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IPlayerService
    {
        Player LoadPlayer(ulong id, ulong serverId);
        Player LoadPlayer(string id, ulong serverId);
        Enemy Walk(Player player, Element direction);
        void SavePlayer(Player player);
        bool AddItem(Player player, Item item);
        bool AddResource(Player player, Resource item);
        void AddExp(Player player, int exp);
        List<Player> GetPlayers(ulong serverId);
    }
    public class PlayerService : IPlayerService
    {
        private readonly IMapService _mapService;
        private readonly IConfiguration _configuration;
        public PlayerService(IMapService mapService, IConfiguration configuration)
        {
            _mapService = mapService;
            _configuration = configuration;
        }
        private string GetID(ulong id, ulong serverId)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(id + "|" + serverId));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public Player LoadPlayer(ulong userid, ulong serverId)
        {
            if (!Directory.Exists("save"))
            {
                Directory.CreateDirectory("save");
            }
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Player>("player");
                var id = GetID(userid, serverId);
                if (!col.Exists((data) => data.Id == id && serverId == data.ServerId))
                {
                    col.Insert(new Player
                    {
                        Id = id,
                        Coin = 0,
                        HP = 20,
                        Atk = 5,
                        Def = 2,
                        Bag = new List<ItemEquip>(),
                        CurrentHP = 20,
                        Exp = 0,
                        Lv = 1,
                        Title = new List<string>(),
                        ServerId = serverId,
                        DiscordId = userid
                    });
                }
                var player = col.FindOne((data) => data.Id == id && serverId == data.ServerId);
                if (player.DiscordId == 0)
                {
                    player.DiscordId = userid;
                }
                return player;
            }
        }

        public void SavePlayer(Player player)
        {
            using (var db = new LiteDatabase("Filename=save\\" + player.ServerId + ".db;connection=shared"))
            {
                var col = db.GetCollection<Player>("player");
                col.Update(player);
            }
        }

        public Enemy Walk(Player player, Element mapType)
        {
            return _mapService.SpawnEnemy(mapType, player.Lv);
        }

        public bool AddItem(Player player, Item item)
        {
            //no duplicate add for equipment
            if (item.UseTimes == -1 && player.Bag.Any(x => x.Name == item.Name))
            {
                return false;
            }
            if (player.Bag.Any(x => x.Name == item.Name))
            {
                player.Bag.Where(x => x.Name == item.Name).First().ItemLeft += item.UseTimes;
            }
            else
            {
                player.Bag.Add(new ItemEquip(item));
            }
            return true;
        }

        public void AddExp(Player player, int exp)
        {
            player.Exp += exp;
            while (player.Exp >= _configuration.LV[player.Lv.ToString()])
            {
                if (_configuration.MaxPlayerLv > player.Lv)
                {
                    player.Lv++;
                    if (player.Lv % 5 == 0)
                    {
                        player.SkillPoint++;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public List<Player> GetPlayers(ulong serverId)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var col = db.GetCollection<Player>("player");
                return col.Find(x => x.ServerId == serverId).ToList();
            }
        }

        public bool AddResource(Player player, Resource item)
        {
            if (item == null)
            {
                return false;
            }
            if (player.ResourceBag == null)
            {
                player.ResourceBag = new List<HoldedResource>();
            }
            if (!player.ResourceBag.Any(x => x.Id == item.Id))
            {
                player.ResourceBag.Add(new HoldedResource(item));
            }
            else
            {
                player.ResourceBag.FirstOrDefault(x => x.Id == item.Id).Amount++;
            }
            return true;
        }

        public Player LoadPlayer(string id, ulong serverId)
        {
            if (!Directory.Exists("save"))
            {
                Directory.CreateDirectory("save");
            }
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Player>("player");
                if (!col.Exists((data) => data.Id == id && serverId == data.ServerId))
                {
                    col.Insert(new Player
                    {
                        Id = id,
                        Coin = 0,
                        HP = 20,
                        Atk = 5,
                        Def = 2,
                        Bag = new List<ItemEquip>(),
                        CurrentHP = 20,
                        Exp = 0,
                        Lv = 1,
                        Title = new List<string>(),
                        ServerId = serverId,
                    });
                }
                var player = col.FindOne((data) => data.Id == id && serverId == data.ServerId);
                return player;
            }
        }
    }
}
