using Newtonsoft.Json;

namespace MyaDiscordBot.Models
{
    public class Items : List<Item>
    {

    }

    public class Item : ICloneable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        //道具/裝備名
        public string Name { get; set; }
        //稀有排名，打怪或者購買裝備的時候會根據依個判定
        public int Rank { get; set; }
        //價錢，如果0表示可能只可以打怪先會跌
        public int Price { get; set; }
        //提升玩家的攻擊力
        public int Atk { get; set; }
        //提升玩家防御力
        public int Def { get; set; }
        //最多使用次數，-1代表無限
        public int UseTimes { get; set; }
        //屬性相剋
        public Element Element { get; set; }
        //道具定義，係裝備or道具
        public ItemType Type { get; set; }
        //增加血量（由於遊戲無max health, 所以可以當補血使用
        public int HP { get; set; }
        //特別技能
        public Ability Ability { get; set; }

        public object Clone()
        {
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(this), deserializeSettings);
        }
    }

    public class ItemEquip : Item
    {
        public ItemEquip()
        {

        }
        public ItemEquip(Item i)
        {
            var item = (Item)i.Clone();
            Name = item.Name;
            Rank = item.Rank;
            Price = item.Price;
            Atk = item.Atk;
            Def = item.Def;
            UseTimes = item.UseTimes;
            Element = item.Element;
            Type = item.Type;
            HP = item.HP;
            Ability = item.Ability;
            ItemLeft = item.UseTimes;
        }

        public int ItemLeft { get; set; }
        public bool IsEquiped { get; set; }
    }

    public enum Element
    {
        Light = 0,
        Dark = 1,
        Fire = 2,
        Wind = 3,
        Earth = 4,
        Water = 5,
        God = 6
    }

    public enum ItemType
    {
        Equipment = 0,
        Item = 1
    }

    public enum Ability
    {
        None = 0,
        WalkOnWater = 1,
        WalkOnLava = 2,
    }
}
