using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface ISpawnerService
    {
        Enemy Spawn(MapItem tile);
    }
    public class SpawnerService
    {

    }
}
