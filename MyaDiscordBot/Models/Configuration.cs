using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public interface IConfiguration 
    {
        string Token { get; set; }
    }

    public class Configuration:IConfiguration
    {
        public string Token { get; set; }
    }
}
