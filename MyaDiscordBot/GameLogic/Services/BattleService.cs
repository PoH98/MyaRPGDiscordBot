using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IBattleService
    {
        BattleResult Battle(Player enemy, Player player);
        BattleResult Battle(Enemy enemy, Player player);
    }
    public class BattleService : IBattleService
    {
        private Random rnd = new Random();
        public BattleResult Battle(Enemy enemy, Player player)
        {
            var result = new BattleResult();
            int battleLoop = 0;
            do
            {
                var atk = player.Atk;
                if (atk < 1)
                {
                    atk = 1;
                }
                var elementWin = ElementDmg(player, enemy);
                if (elementWin > 0 || elementWin == -2)
                {
                    atk = (int)Math.Round(atk * 1.5);
                }
                else if (elementWin == -1)
                {
                    atk = (int)Math.Round(atk / 1.5);
                }
                if (player.Bag.Count > 0)
                {
                    var item = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Critical);
                    if (item != null)
                    {
                        var rate = rnd.NextDouble();
                        if (item.AbilityRate >= rate)
                        {
                            //critical!
                            atk = (int)Math.Round(atk * 1.5);
                        }
                    }
                }
                atk -= enemy.Def;
                if (atk < 0)
                {
                    atk = 0;
                }
                if (player.Bag.Count > 0)
                {
                    var item = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Heal);
                    if (item != null)
                    {
                        player.CurrentHP += (int)Math.Round(atk * item.AbilityRate);
                    }
                }
                enemy.HP -= atk;
                result.DamageDealt += atk;
                if (enemy.HP > 0)
                {
                    atk = enemy.Atk;
                    if (elementWin == -1 || elementWin == -2)
                    {
                        atk = (int)Math.Round(atk * 1.2);
                    }
                    else if (elementWin > 0)
                    {
                        atk = (int)Math.Round(atk / 1.2);
                    }
                    atk -= player.Def;
                    if (atk < 0)
                    {
                        atk = 0;
                    }
                    if (player.Bag.Count > 0)
                    {
                        var item = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Immune);
                        if (item != null)
                        {
                            var rate = rnd.NextDouble();
                            if (item.AbilityRate >= rate)
                            {
                                //immune
                                atk = 0;
                            }
                        }
                    }
                    player.CurrentHP -= atk;
                    result.DamageReceived += atk;
                }
                //awaiting items function to be done, suppose use itemService to calculate how many Items should be used and add calculation on equipment
                if (player.CurrentHP > 0)
                {
                    var items = player.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                    if (items.Any())
                    {
                        //use items
                        while ((player.CurrentHP <= enemy.Atk || player.CurrentHP < 5) && player.CurrentHP < player.HP && items.Any(x => x.HP > 0))
                        {
                            var item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
                            item.ItemLeft--;
                            player.CurrentHP += item.HP;
                            //add max HP forever, but limit a rate
                            if (player.HP < 90 && player.Lv < 40)
                            {
                                player.HP += item.HP / 5;
                            }
                            else if (player.HP < 200 && player.Lv < 60)
                            {
                                player.HP += item.HP / 10;
                            }
                            if (player.CurrentHP > player.HP)
                            {
                                player.CurrentHP = player.HP;
                            }
                            if (result.ItemsUsed.Any(x => x.Key == item))
                            {
                                result.ItemsUsed[item]++;
                            }
                            else
                            {
                                result.ItemsUsed.Add(item, 1);
                            }
                        }
                    }
                }
                battleLoop++;
            }
            while (player.CurrentHP > 0 && enemy.HP > 0 && battleLoop <= 1000);
            if (player.CurrentHP > 0 && enemy.HP <= 0)
            {
                result.IsVictory = true;
            }
            else
            {
                double wait = player.HP * 3.5;
                if (wait > 120)
                {
                    wait = 120;
                }
                player.NextCommand = DateTime.Now.AddMinutes(wait);
            }
            return result;
        }

        public BattleResult Battle(Player enemy, Player player)
        {
            var result = new BattleResult();
            int battleLoop = 0;
            do
            {
                var atk = player.Atk;
                if (atk < 1)
                {
                    atk = 1;
                }
                var elementWin = ElementDmg(player, enemy);
                if (elementWin > 0 || elementWin == -2)
                {
                    atk = (int)Math.Round(atk * 1.5);
                }
                else if (elementWin < 0)
                {
                    atk = (int)Math.Round(atk * 0.5);
                }
                if (player.Bag.Count > 0)
                {
                    var item = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Critical);
                    if (item != null)
                    {
                        var rate = rnd.NextDouble();
                        if (item.AbilityRate >= rate)
                        {
                            //critical!
                            atk = (int)Math.Round(atk * 1.5);
                        }
                    }
                }
                atk -= enemy.Def;
                if (atk < 0)
                {
                    atk = 0;
                }
                if (player.Bag.Count > 0)
                {
                    var item = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Heal);
                    if (item != null)
                    {
                        player.CurrentHP += (int)Math.Round(atk * item.AbilityRate);
                    }
                }
                if (enemy.Bag.Count > 0)
                {
                    var item = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Immune);
                    if (item != null)
                    {
                        var rate = rnd.NextDouble();
                        if (item.AbilityRate >= rate)
                        {
                            //immune
                            atk = 0;
                        }
                    }
                }
                enemy.CurrentHP -= atk;
                if (enemy.HP > 0)
                {
                    atk = enemy.Atk;
                    if (elementWin == -1 || elementWin == -2)
                    {
                        atk = (int)Math.Round(atk * 1.5);
                    }
                    else if (elementWin > 0)
                    {
                        atk = (int)Math.Round(atk * 0.5);
                    }
                    if (enemy.Bag.Count > 0)
                    {
                        var item = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Critical);
                        if (item != null)
                        {
                            var rate = rnd.NextDouble();
                            if (item.AbilityRate >= rate)
                            {
                                //critical!
                                atk = (int)Math.Round(atk * 1.5);
                            }
                        }
                    }
                    atk -= player.Def;
                    if (atk < 0)
                    {
                        atk = 0;
                    }
                    if (enemy.Bag.Count > 0)
                    {
                        var item = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Heal);
                        if (item != null)
                        {
                            enemy.CurrentHP += (int)Math.Round(atk * item.AbilityRate);
                        }
                    }
                    if (player.Bag.Count > 0)
                    {
                        var item = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Immune);
                        if (item != null)
                        {
                            var rate = rnd.NextDouble();
                            if (item.AbilityRate >= rate)
                            {
                                //immune
                                atk = 0;
                            }
                        }
                    }
                    player.CurrentHP -= atk;
                }
                //awaiting items function to be done, suppose use itemService to calculate how many Items should be used and add calculation on equipment
                if (player.CurrentHP > 0)
                {
                    var items = player.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                    if (items.Count() > 0)
                    {
                        //use items
                        while ((player.CurrentHP <= enemy.Atk || player.CurrentHP < 5) && player.CurrentHP < player.HP && items.Any(x => x.HP > 0))
                        {
                            var item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
                            item.ItemLeft--;
                            if (player.HP < 90 && player.Lv < 40)
                            {
                                player.HP += item.HP / 5;
                            }
                            else if (player.HP < 200 && player.Lv < 60)
                            {
                                player.HP += item.HP / 10;
                            }
                            //heal
                            player.CurrentHP += item.HP;
                            if (player.CurrentHP > player.HP)
                            {
                                player.CurrentHP = player.HP;
                            }
                        }
                    }
                }
                if (enemy.CurrentHP > 0)
                {
                    var items = enemy.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                    if (items.Count() > 0)
                    {
                        //use items
                        while ((enemy.CurrentHP <= player.Atk || enemy.CurrentHP < 5) && enemy.CurrentHP < enemy.HP && items.Any(x => x.HP > 0))
                        {
                            var item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
                            item.ItemLeft--;
                            if (enemy.HP < 90 && enemy.Lv < 40)
                            {
                                enemy.HP += item.HP / 5;
                            }
                            else if (enemy.HP < 200 && enemy.Lv < 60)
                            {
                                enemy.HP += item.HP / 10;
                            }
                            //heal
                            enemy.CurrentHP += item.HP;
                            if (enemy.CurrentHP > enemy.HP)
                            {
                                enemy.CurrentHP = enemy.HP;
                            }
                        }
                    }
                }
                battleLoop++;
            }
            while (player.CurrentHP > 0 && enemy.CurrentHP > 0 && battleLoop < 1000);
            if (player.CurrentHP > 0 && enemy.CurrentHP <= 0 || player.CurrentHP > enemy.CurrentHP)
            {
                result.IsVictory = true;
            }
            return result;
        }


        /// <summary>
        /// > 0 means player win, 0 means no extra dmg and nothing happens, < 0 means player lose, but for light & dark will return -2
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private int ElementDmg(Player player, Enemy enemy)
        {
            switch (enemy.Element)
            {
                case Element.Fire:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Water).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Wind).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Wind:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Fire).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Earth).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Water:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Earth).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Fire).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Earth:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Wind).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Water).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Dark:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Light).Count() > 0)
                    {
                        //player win, but special case
                        return -2;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Dark).Count() > 0)
                    {
                        //player GG
                        return 0;
                    }
                    return -1;
                case Element.Light:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Dark).Count() > 0)
                    {
                        //player win, but special case
                        return -2;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Light).Count() > 0)
                    {
                        //player GG
                        return 0;
                    }
                    return -1;
                //god Element
                default:
                    //player must GG
                    return -1;

            }
        }

        private int ElementDmg(Player player, Player enemy)
        {
            if (enemy.Bag.Where(x => x.IsEquiped).Count() < 1)
            {
                //enemy nothing equiped
                return 0;
            }
            switch (enemy.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Type != ItemType.指環).First().Element)
            {
                case Element.Fire:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Water).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Wind).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Wind:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Fire).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Earth).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Water:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Earth).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Fire).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Earth:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Wind).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Water).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Dark:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Light).Count() > 0)
                    {
                        //player win, but special case
                        return -2;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Dark).Count() > 0)
                    {
                        //player GG
                        return 0;
                    }
                    return -1;
                case Element.Light:
                    if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Dark).Count() > 0)
                    {
                        //player win, but special case
                        return -2;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Element == Element.Light).Count() > 0)
                    {
                        //player GG
                        return 0;
                    }
                    return -1;
                //god Element
                default:
                    //player must GG
                    return -1;

            }
        }
    }
}
