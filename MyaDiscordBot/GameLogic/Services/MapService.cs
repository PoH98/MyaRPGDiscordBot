using LiteDB;
using MyaDiscordBot.Models;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IMapService
    {
        Map GetCurrentMap(ulong serverId);
        void NextStage(int serverId);
        Enemy SpawnEnemy(Coordinate coordinate, Map map, int stage);
        Stream GetMap(Coordinate coordinate, Map currentMap);
        int CurrentStage(ulong serverId);
    }
    internal class MapService : IMapService
    {
        private readonly Maps maps;
        private readonly ISpawnerService spawnerService;
        public MapService(Maps maps, ISpawnerService spawnerService)
        {
            this.maps = maps;
            this.spawnerService = spawnerService;
        }

        public int CurrentStage(ulong serverId)
        {
            if (!Directory.Exists("save"))
            {
                Directory.CreateDirectory("save");
            }
            if (!File.Exists(System.IO.Path.Combine("save", serverId + ".json")))
            {
                File.WriteAllText(System.IO.Path.Combine("save", serverId + ".json"), JsonConvert.SerializeObject(new Save
                {
                    CurrentStage = 1,
                    CurrentBoss = null,
                    EnemyLeft = 15000
                }));
            }
            var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(System.IO.Path.Combine("save", serverId + ".json")));
            return save.CurrentStage;
        }

        public Map GetCurrentMap(ulong serverId)
        {
            return maps[CurrentStage(serverId) - 1];
        }

        public Stream GetMap(Coordinate coordinate, Map currentMap)
        {
            MemoryStream stream = new MemoryStream();
            Image img = new Image<Rgba32>(currentMap.MapData[0].Count * 20, currentMap.MapData.Count * 20);
            Image land = Image.Load(File.ReadAllBytes("Assets\\grass.jpg"));
            Image lava = Image.Load(File.ReadAllBytes("Assets\\lava.jpg"));
            Image water = Image.Load(File.ReadAllBytes("Assets\\water.jpg"));
            Image rock = Image.Load(File.ReadAllBytes("Assets\\rock.jpg"));
            for (var x = 0; x < currentMap.MapData[0].Count; x++)
            {
                for (var y = 0; y < currentMap.MapData.Count; y++)
                {
                    var rect = new RectangularPolygon(new RectangleF(x * 20, y * 20, 20, 20));
                    switch (currentMap.MapData[y][x])
                    {
                        case MapItem.PlayerSpawn:
                            img.Mutate(x => x.Fill(Color.MediumPurple, rect));
                            break;
                        case MapItem.Lava:
                            img.Mutate(z => z.DrawImage(lava, new Point(x * 20, y * 20), 1));
                            break;
                        case MapItem.Water:
                            img.Mutate(z => z.DrawImage(water, new Point(x * 20, y * 20), 1));
                            break;
                        case MapItem.Land:
                            img.Mutate(z => z.DrawImage(land, new Point(x * 20, y * 20), 1));
                            break;
                        case MapItem.Wall:
                            img.Mutate(z => z.DrawImage(rock, new Point(x * 20, y * 20), 1));
                            break;
                    }
                    if (coordinate.X == x && coordinate.Y == y)
                    {
                        var circle = new EllipsePolygon(x * 20 + 10, y * 20 + 10, 20, 20);
                        img.Mutate(x => x.Fill(Color.Yellow, circle));
                    }
                }
            }
            img.Save(stream, new JpegEncoder());
            return stream;
        }

        public void NextStage(int serverId)
        {
            var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(serverId + ".json"));
            save.CurrentStage++;
            if (save.CurrentStage >= maps.Count)
            {
                save.CurrentStage = 1;
            }
            File.WriteAllText(serverId + ".json", JsonConvert.SerializeObject(save));
        }

        public Enemy SpawnEnemy(Coordinate coordinate, Map map, int stage)
        {
            Enemy result = null;
            var tiles = GetTiles(coordinate, map);
            if (tiles.Count() == 1)
            {
                if (tiles.First().Tile == MapItem.Wall)
                {
                    throw new ArgumentException("Hacker found! How the fuck you get into between walls");
                }
                result = spawnerService.Spawn(stage, tiles.First().Tile);
            }
            else
            {
                result = spawnerService.Spawn(stage, tiles.Select(x => x.Tile).ToArray());
            }
            return result;
        }

        private IEnumerable<GroupedTiles> GetTiles(Coordinate coordinate, Map map)
        {
            //check 3x3 region if exist other than land, if so calculate spawn specific element enemies
            var nineX = new List<MapItem>();
            //[x][ ][ ]
            //[ ][0][ ]
            //[ ][ ][ ]
            if (coordinate.Y - 1 >= 0 && coordinate.X - 1 > 0)
            {
                nineX.Add(map.MapData[coordinate.Y - 1][coordinate.X - 1]);
            }
            //[ ][x][ ]
            //[ ][0][ ]
            //[ ][ ][ ]
            if (coordinate.Y - 1 >= 0)
            {
                nineX.Add(map.MapData[coordinate.Y - 1][coordinate.X]);
            }
            //[ ][ ][x]
            //[ ][0][ ]
            //[ ][ ][ ]
            if (coordinate.Y - 1 >= 0 && map.MapData[coordinate.Y].Count > coordinate.X)
            {
                nineX.Add(map.MapData[coordinate.Y - 1][coordinate.X + 1]);
            }
            //[ ][ ][ ]
            //[x][0][ ]
            //[ ][ ][ ]
            if (coordinate.X - 1 >= 0)
            {
                nineX.Add(map.MapData[coordinate.Y][coordinate.X - 1]);
            }
            //[ ][ ][ ]
            //[ ][0][x]
            //[ ][ ][ ]
            if (map.MapData[coordinate.Y].Count > coordinate.X + 1)
            {
                nineX.Add(map.MapData[coordinate.Y][coordinate.X + 1]);
            }
            //[ ][ ][ ]
            //[ ][0][ ]
            //[x][ ][ ]
            if (map.MapData.Count > coordinate.Y + 1 && coordinate.X - 1 >= 0)
            {
                nineX.Add(map.MapData[coordinate.Y + 1][coordinate.X - 1]);
            }
            //[ ][ ][ ]
            //[ ][0][ ]
            //[ ][x][ ]
            if (map.MapData.Count > coordinate.Y + 1)
            {
                nineX.Add(map.MapData[coordinate.Y + 1][coordinate.X]);
            }
            //[ ][ ][ ]
            //[ ][0][ ]
            //[ ][ ][x]
            if (map.MapData.Count > coordinate.Y + 1 && map.MapData[coordinate.Y].Count > coordinate.X + 1)
            {
                nineX.Add(map.MapData[coordinate.Y + 1][coordinate.X + 1]);
            }
            //group all tiles and calculate counts
            return nineX.GroupBy(x => x).Select(group => new GroupedTiles { Tile = group.Key, Count = group.Count() }).OrderBy(x => x.Count);
        }
    }
}
