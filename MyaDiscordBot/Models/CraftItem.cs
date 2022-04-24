using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class CraftItems:List<CraftItem>
    {

    }

    public class CraftItem:Item
    {
        public List<Item> RequiredItems { get; set; }
    }
}
