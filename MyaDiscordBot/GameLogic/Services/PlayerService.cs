using LiteDB;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IPlayerService
    {
        Player LoadPlayer(ulong id, ulong serverId);

        Enemy Walk(Player player, int direction);
        Task SavePlayer(Player player);
    }
    public class PlayerService : IPlayerService
    {
        private readonly IMapService _mapService;
        public PlayerService(IMapService mapService)
        {
            _mapService = mapService;
        }
        public Player LoadPlayer(ulong id, ulong serverId)
        {
            using (var db = new LiteDatabase(@"data.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Player>("player");
                if(!col.Exists((data) => data.Id == id && serverId == data.ServerId))
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

        public Task SavePlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public Enemy Walk(Player player, int direction)
        {
            player.NextCommand = DateTime.Now.AddMinutes(15);
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
                    player.Coordinate.X--;
                    break;
                case 4:
                    player.Coordinate.X++;
                    break;
            }
            var map = _mapService.GetCurrentMap(player.ServerId);
            var mapPoint = map.MapData[player.Coordinate.X][player.Coordinate.Y];
            if (mapPoint != MapItem.Land)
            {
                //check allowed to walk on water, more to come
                if ((!player.Bag.Any(x => x.Ability == Ability.WalkOnWater && x.IsEquiped) && mapPoint == MapItem.Water))
                {
                    //not walkable, revert move
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
                            player.Coordinate.X++;
                            break;
                        case 4:
                            player.Coordinate.X--;
                            break;
                    }
                }
            }
            return null;
        }
    }
}
