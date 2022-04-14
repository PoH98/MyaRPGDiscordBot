using LiteDB;
using MyaDiscordBot.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IPlayerService
    {
        Player LoadPlayer(ulong id, ulong serverId);
        Enemy Walk(Player player, Element direction);
        void SavePlayer(Player player);
        bool AddItem(Player player, Item item);
        void AddExp(Player player, int exp);
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
                        Def = 5,
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
                player.Bag.Where(x => x.Name == item.Name).First().ItemLeft++;
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
            var lvUpRequired = _configuration.LV[player.Lv.ToString()];
            if (player.Exp >= lvUpRequired)
            {
                player.Lv++;
            }
        }
    }
}
