using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IBattleService
    {
        BattleResult Battle(Enemy enemy, Player player);
        Item GetReward(Enemy enemy, Player player);
    }
    public class BattleService : IBattleService
    {
        private readonly Items items;
        public BattleService(Items items)
        {
            this.items = items;
        }
        public BattleResult Battle(Enemy enemy, Player player)
        {
            var result = new BattleResult();
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
                atk -= enemy.Def;
                if (atk < 0)
                {
                    atk = 0;
                }
                enemy.HP -= atk;
                result.DamageDealt += player.Atk - enemy.Def;
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
                    player.CurrentHP -= atk;
                    result.DamageReceived += atk;
                }
                //awaiting items function to be done, suppose use itemService to calculate how many Items should be used and add calculation on equipment
                var items = player.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                if (items.Count() > 0)
                {
                    //use items
                    if ((player.HP <= enemy.Atk || player.HP < 5) && items.Any(x => x.HP > 0))
                    {
                        var item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
                        item.ItemLeft--;
                        player.HP += item.HP;
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
            while (player.CurrentHP > 0 && enemy.HP > 0);
            if (player.CurrentHP > 0 && enemy.HP <= 0)
            {
                result.IsVictory = true;
            }
            else
            {
                double wait = player.HP * 3.5;
                player.NextCommand = DateTime.Now.AddMinutes(wait);
            }
            return result;
        }

        public Item GetReward(Enemy enemy, Player player)
        {
            Random rnd = new Random();
            decimal i = (decimal)rnd.NextDouble();
            if (i <= enemy.ItemDropRate)
            {
                var reward = items.Where(x => enemy.DropRank.Any(y => y == x.Rank) && enemy.Element == x.Element && !player.Bag.Any(y => y.Id == x.Id)).ToList();
                if (reward.Any())
                {
                    double cumulSum = 0;
                    int cnt = reward.Count();
                    for (int slot = 0; slot < cnt; slot++)
                    {
                        cumulSum += items[slot].DropRate;
                        items[slot].DropRate = cumulSum;
                    }
                    double divSpot = rnd.NextDouble() * cumulSum;
                    return reward.FirstOrDefault(i => i.DropRate >= divSpot);
                }
            }
            return null;
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
                    if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Water).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Wind).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Wind:
                    if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Fire).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Earth).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Water:
                    if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Earth).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Fire).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Earth:
                    if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Wind).Count() > 0)
                    {
                        //player win
                        return 1;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Water).Count() > 0)
                    {
                        //player GG
                        return -1;
                    }
                    return 0;
                case Element.Dark:
                    if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Light).Count() > 0)
                    {
                        //player win, but special case
                        return -2;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Dark).Count() > 0)
                    {
                        //player GG
                        return 0;
                    }
                    return -1;
                case Element.Light:
                    if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Dark).Count() > 0)
                    {
                        //player win, but special case
                        return -2;
                    }
                    else if (player.Bag.Where(x => x.IsEquiped && x.Atk > 0 && x.Element == Element.Light).Count() > 0)
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
