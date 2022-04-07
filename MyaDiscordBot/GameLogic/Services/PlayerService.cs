using LiteDB;
using MyaDiscordBot.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IPlayerService
    {
        Player LoadPlayer(ulong id, ulong serverId);
        Enemy Walk(Player player, long direction);
        void SavePlayer(Player player);
    }
    public class PlayerService : IPlayerService
    {
        private readonly IMapService _mapService;
        public PlayerService(IMapService mapService)
        {
            _mapService = mapService;
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
                        Coordinate = _mapService.GetCurrentMap(serverId).SpawnCoordinate,
                        HighestHP = 20,
                        HighestDef = 5,
                        HighestDmg = 5
                    });
                }
                return col.FindOne((data) => data.Id == id && serverId == data.ServerId);
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

        public Enemy Walk(Player player, long direction)
        {
            var map = _mapService.GetCurrentMap(player.ServerId);
            if (_mapService.CurrentStage(player.ServerId) != player.CurrentStage)
            {
                //reset player position
                player.Coordinate = map.SpawnCoordinate;
                player.CurrentStage = _mapService.CurrentStage(player.ServerId);
                if (player.CurrentStage == 1)
                {
                    //all done, reset all and give myacoin
                }
            }
            switch (direction)
            {
                case 1:
                    //foward
                    player.Coordinate.Y--;
                    break;
                case 2:
                    //backward
                    player.Coordinate.Y++;
                    break;
                case 3:
                    //left
                    player.Coordinate.X--;
                    break;
                case 4:
                    player.Coordinate.X++;
                    break;
            }
            //out of map
            if (player.Coordinate.Y > map.MapData.Count - 1 || player.Coordinate.Y < 0 || player.Coordinate.X < 0 || player.Coordinate.X > map.MapData[player.Coordinate.Y].Count)
            {
                //not walkable, revert move
                switch (direction)
                {
                    case 1:
                        //foward
                        player.Coordinate.Y++;
                        break;
                    case 2:
                        //backward
                        player.Coordinate.Y--;
                        break;
                    case 3:
                        //left
                        player.Coordinate.X++;
                        break;
                    case 4:
                        player.Coordinate.X--;
                        break;
                }
            }
            var mapPoint = map.MapData[player.Coordinate.Y][player.Coordinate.X];
            if (mapPoint != MapItem.Land)
            {
                //check allowed to walk on water, more to come
                if ((!player.Bag.Any(x => x.Ability == Ability.WalkOnWater && x.IsEquiped) && mapPoint == MapItem.Water) ||
                    mapPoint == MapItem.Lava || mapPoint == MapItem.Wall)
                {
                    //not walkable, revert move
                    switch (direction)
                    {
                        case 1:
                            //foward
                            player.Coordinate.Y++;
                            break;
                        case 2:
                            //backward
                            player.Coordinate.Y--;
                            break;
                        case 3:
                            //left
                            player.Coordinate.X++;
                            break;
                        case 4:
                            player.Coordinate.X--;
                            break;
                    }
                }
            }
            return _mapService.SpawnEnemy(player.Coordinate, map, player.CurrentStage);
        }
    }
}
