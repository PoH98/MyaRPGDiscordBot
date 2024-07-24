using Discord;
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
        private readonly Random rnd = new();
        public BattleResult Battle(Enemy enemy, Player player)
        {
            BattleResult result = new();
            int battleLoop = 0;
            //cap max values when fighting between enemy
            int oriAtk = player.Atk;
            int oriDef = player.Def;
            if (player.Atk >= 200)
            {
                player.Atk = 200;
            }
            if (player.Def >= 50)
            {
                player.Def = 50;
            }
            bool useSkill = player.Bag.Count > 0;
            ItemEquip debuffer = null, critical = null, heal = null, immune = null, reflect = null;
            if (useSkill)
            {
                debuffer = player.Bag.FirstOrDefault(x => x.Type == ItemType.指環 && x.Ability == Ability.DebuffStates && x.IsEquiped);
                critical = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Critical);
                heal = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Heal);
                immune = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Immune);
                reflect = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Reflect);
            }
            bool lockSkill = player.Bag.Any(x => x.Type == ItemType.指環 && x.Ability == Ability.DebuffSkill && x.IsEquiped);
            if(debuffer != null)
            {
                enemy.Atk -= (int)Math.Round(enemy.Atk * debuffer.AbilityRate);
                enemy.Def -= (int)Math.Round(enemy.Def * debuffer.AbilityRate);
            }
            do
            {
                int atk = player.Atk;
                if (atk < 1)
                {
                    atk = 1;
                }
                int elementWin = ElementDmg(player, enemy);
                if (elementWin is > 0 or (-2))
                {
                    atk = (int)Math.Round(atk * 1.5);
                }
                else if (elementWin == -1)
                {
                    atk = (int)Math.Round(atk / 1.5);
                }
                if (critical != null)
                {
                    double rate = rnd.NextDouble();
                    if (critical.AbilityRate >= rate)
                    {
                        //critical!
                        atk = (int)Math.Round(atk * 1.5);
                    }
                }
                atk -= enemy.Def;
                if (atk < 1)
                {
                    atk = 1;
                }
                if (heal != null)
                {
                    player.CurrentHP += (int)Math.Round(atk * heal.AbilityRate);
                }
                enemy.HP -= atk;
                result.DamageDealt += atk;
                if (enemy.HP > 0)
                {
                    atk = enemy.Atk;
                    if (elementWin is (-1) or (-2))
                    {
                        atk = (int)Math.Round(atk * 1.2);
                    }
                    else if (elementWin > 0)
                    {
                        atk = (int)Math.Round(atk / 1.2);
                    }
                    atk -= player.Def;
                    if (atk < 1)
                    {
                        atk = 1;
                    }
                    if (immune != null)
                    {
                        double rate = rnd.NextDouble();
                        if (immune.AbilityRate >= rate)
                        {
                            //immune
                            atk = 0;
                        }
                    }
                    if (reflect != null)
                    {
                        double rate = rnd.NextDouble();
                        if (reflect.AbilityRate >= rate)
                        {
                            //reflect
                            enemy.HP -= atk + player.Def;
                            atk = 0;
                        }
                    }
                    player.CurrentHP -= atk;
                    result.DamageReceived += atk;
                }
                //awaiting items function to be done, suppose use itemService to calculate how many Items should be used and add calculation on equipment
                if (player.CurrentHP > 0)
                {
                    IEnumerable<ItemEquip> items = player.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                    if (items.Any())
                    {
                        //use items
                        while ((player.CurrentHP <= enemy.Atk || player.CurrentHP < 5) && player.CurrentHP < player.HP && items.Any(x => x.HP > 0))
                        {
                            ItemEquip item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
                            item.ItemLeft--;
                            player.CurrentHP += item.HP;
                            //add max HP forever, but limit a rate
                            if (player.HP < 90 && player.Lv < 40)
                            {
                                player.HP += item.HP / 5;
                            }
                            else if (player.HP < 100 && player.Lv < 60)
                            {
                                player.HP += item.HP / 10;
                            }
                            else if(player.HP < 200)
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
            player.Atk = oriAtk;
            player.Def = oriDef;
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
            BattleResult result = new();
            int battleLoop = 0;
            bool playerUseSkill = player.Bag.Count > 0;
            ItemEquip playerDebuffer = null, playerCritical = null, playerHeal = null, playerImmune = null, playerReflect = null, playerCopyCat = null;
            if (playerUseSkill)
            {
                playerDebuffer = player.Bag.FirstOrDefault(x => x.Type == ItemType.指環 && x.Ability == Ability.DebuffStates && x.IsEquiped);
                playerCritical = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Critical);
                playerHeal = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Heal);
                playerImmune = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Immune);
                playerReflect = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Reflect);
                playerCopyCat = player.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.CopyCat);
            }
            bool playerLockSkill = player.Bag.Any(x => x.Type == ItemType.指環 && x.Ability == Ability.DebuffSkill && x.IsEquiped);
            bool enemyUseSkill = enemy.Bag.Count > 0;
            ItemEquip enemyDebuffer = null, enemyCritical = null, enemyHeal = null, enemyImmune = null, enemyReflect = null, enemyCopyCat = null;
            if (enemyUseSkill)
            {
                enemyDebuffer = enemy.Bag.FirstOrDefault(x => x.Type == ItemType.指環 && x.Ability == Ability.DebuffStates && x.IsEquiped);
                enemyCritical = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Critical);
                enemyHeal = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Heal);
                enemyImmune = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Immune);
                enemyReflect = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.Reflect);
                enemyCopyCat = enemy.Bag.FirstOrDefault(x => x.IsEquiped && x.Type == ItemType.指環 && x.Ability == Ability.CopyCat);
            }
            bool enemyLockSkill = enemy.Bag.Any(x => x.Type == ItemType.指環 && x.Ability == Ability.DebuffSkill && x.IsEquiped);
            do
            {
                int atk = player.Atk;
                if (atk < 1)
                {
                    atk = 1;
                }
                int elementWin = ElementDmg(player, enemy);
                if (elementWin is > 0 or (-2))
                {
                    atk = (int)Math.Round(atk * 1.5);
                }
                else if (elementWin < 0)
                {
                    atk = (int)Math.Round(atk * 0.5);
                }
                if(player.MarriedUser != 0)
                {
                    atk = (int)Math.Round(atk * 1.1);
                }
                if ((playerCritical != null && !enemyLockSkill) || (playerCopyCat != null && enemyCritical != null))
                {
                    double rate = rnd.NextDouble();
                    if(playerCopyCat != null)
                    {
                        if (enemyCritical.AbilityRate >= rate)
                        {
                            //critical!
                            atk = (int)Math.Round(atk * 1.5);
                        }
                    }
                    else
                    {
                        if (playerCritical.AbilityRate >= rate)
                        {
                            //critical!
                            atk = (int)Math.Round(atk * 1.5);
                        }
                    }
                }
                var def = enemy.Def;
                if ((enemyDebuffer != null && !playerLockSkill) || (enemyCopyCat != null && playerDebuffer != null))
                {
                    if(enemyCopyCat != null)
                    {
                        atk -= (int)Math.Round(atk * playerDebuffer.AbilityRate);
                    }
                    else
                    {
                        atk -= (int)Math.Round(atk * enemyDebuffer.AbilityRate);
                    }
                }
                if ((playerDebuffer != null && !enemyLockSkill) || (playerCopyCat != null && enemyDebuffer != null))
                {
                    if (playerCopyCat != null)
                    {
                        def -= (int)Math.Round(def * enemyDebuffer.AbilityRate);
                    }
                    else
                    {
                        def -= (int)Math.Round(def * playerDebuffer.AbilityRate);
                    }
                }
                atk -= def;
                if (atk < 1)
                {
                    atk = 1;
                }
                if ((playerHeal != null && !enemyLockSkill) || (playerCopyCat != null && enemyHeal != null))
                {
                    if(playerCopyCat != null)
                    {
                        player.CurrentHP += (int)Math.Round(atk * enemyHeal.AbilityRate);
                    }
                    else
                    {
                        player.CurrentHP += (int)Math.Round(atk * playerHeal.AbilityRate);
                    }
                }
                if((enemyImmune != null && !playerLockSkill) || (enemyCopyCat != null && playerImmune != null))
                {
                    double rate = rnd.NextDouble();
                    if(enemyCopyCat != null)
                    {
                        if (playerImmune.AbilityRate >= rate)
                        {
                            //immune
                            atk = 0;
                        }
                    }
                    else
                    {
                        if (enemyImmune.AbilityRate >= rate)
                        {
                            //immune
                            atk = 0;
                        }
                    }
                }
                if((enemyReflect!= null && !playerLockSkill) || (enemyCopyCat != null && playerReflect != null))
                {
                    double rate = rnd.NextDouble();
                    if(enemyCopyCat != null)
                    {
                        if (playerReflect.AbilityRate >= rate)
                        {
                            //reflect
                            player.CurrentHP -= atk + def;
                            atk = 0;
                        }
                    }
                    else
                    {
                        if (enemyReflect.AbilityRate >= rate)
                        {
                            //reflect
                            player.CurrentHP -= atk;
                            atk = 0;
                        }
                    }
                }
                enemy.CurrentHP -= atk;
                if (enemy.CurrentHP > 0)
                {
                    atk = enemy.Atk;
                    if (elementWin is (-1) or (-2))
                    {
                        atk = (int)Math.Round(atk * 1.5);
                    }
                    else if (elementWin > 0)
                    {
                        atk = (int)Math.Round(atk * 0.5);
                    }
                    if ((enemyCritical != null && !playerLockSkill) || (enemyCopyCat != null && playerCritical != null))
                    {
                        double rate = rnd.NextDouble();
                        if(enemyCopyCat != null)
                        {
                            if (playerCritical.AbilityRate >= rate)
                            {
                                //critical!
                                atk = (int)Math.Round(atk * 1.5);
                            }
                        }
                        else
                        {
                            if (enemyCritical.AbilityRate >= rate)
                            {
                                //critical!
                                atk = (int)Math.Round(atk * 1.5);
                            }
                        }
                    }
                    def = player.Def;
                    if ((playerDebuffer != null && !enemyLockSkill) || (playerCopyCat != null && enemyDebuffer != null))
                    {
                        if(playerCopyCat != null)
                        {
                            atk -= (int)Math.Round(atk * enemyDebuffer.AbilityRate);
                        }
                        else
                        {
                            atk -= (int)Math.Round(atk * playerDebuffer.AbilityRate);
                        }
                    }
                    if((enemyDebuffer != null && !playerLockSkill) || (enemyCopyCat != null && playerDebuffer != null))
                    {
                        if(enemyCopyCat != null)
                        {
                            def -= (int)Math.Round(def * playerDebuffer.AbilityRate);
                        }
                        else
                        {
                            def -= (int)Math.Round(def * enemyDebuffer.AbilityRate);
                        }
                    }
                    atk -= def;
                    if (atk < 0)
                    {
                        atk = 0;
                    }
                    if ((enemyHeal != null && !playerLockSkill) || (enemyCopyCat != null && playerHeal != null))
                    {
                        if(enemyCopyCat != null)
                        {
                            enemy.CurrentHP += (int)Math.Round(atk * playerHeal.AbilityRate);
                        }
                        else
                        {
                            enemy.CurrentHP += (int)Math.Round(atk * enemyHeal.AbilityRate);
                        }
                    }
                    if((playerImmune != null && !enemyLockSkill) || (playerCopyCat != null && enemyImmune != null))
                    {
                        double rate = rnd.NextDouble();
                        if(playerCopyCat != null)
                        {
                            if (enemyImmune.AbilityRate >= rate)
                            {
                                //immune
                                atk = 0;
                            }
                        }
                        else
                        {
                            if (playerImmune.AbilityRate >= rate)
                            {
                                //immune
                                atk = 0;
                            }
                        }
                    }
                    if((playerReflect != null && !enemyLockSkill) || (playerCopyCat != null && enemyReflect != null))
                    {
                        double rate = rnd.NextDouble();
                        if(playerCopyCat != null)
                        {
                            if (enemyReflect.AbilityRate >= rate)
                            {
                                //reflect
                                enemy.CurrentHP -= atk + def;
                                atk = 0;
                            }
                        }
                        else
                        {
                            if (playerReflect.AbilityRate >= rate)
                            {
                                //reflect
                                enemy.CurrentHP -= atk + def;
                                atk = 0;
                            }
                        }
                    }
                    player.CurrentHP -= atk;
                }
                //awaiting items function to be done, suppose use itemService to calculate how many Items should be used and add calculation on equipment
                if (player.CurrentHP > 0)
                {
                    IEnumerable<ItemEquip> items = player.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                    if (items.Count() > 0)
                    {
                        //use items
                        while ((player.CurrentHP <= enemy.Atk || player.CurrentHP < 5) && player.CurrentHP < player.HP && items.Any(x => x.HP > 0))
                        {
                            ItemEquip item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
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
                    IEnumerable<ItemEquip> items = enemy.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                    if (items.Count() > 0)
                    {
                        //use items
                        while ((enemy.CurrentHP <= player.Atk || enemy.CurrentHP < 5) && enemy.CurrentHP < enemy.HP && items.Any(x => x.HP > 0))
                        {
                            ItemEquip item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
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
            if ((player.CurrentHP > 0 && enemy.CurrentHP <= 0) || player.CurrentHP > enemy.CurrentHP)
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
            ItemEquip items = enemy.Bag.Where(x => x.IsEquiped && x.Type != ItemType.道具 && x.Type != ItemType.指環)?.First();
            if (items == null)
            {
                return 1;
            }
            switch (items.Element)
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
