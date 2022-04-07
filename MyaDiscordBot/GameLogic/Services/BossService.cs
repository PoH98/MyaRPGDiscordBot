using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IBossService
    {
        Enemy GetBoss();
    }
    public class BossService : IBossService
    {
        public Enemy GetBoss()
        {
            throw new NotImplementedException();
        }
    }
}
