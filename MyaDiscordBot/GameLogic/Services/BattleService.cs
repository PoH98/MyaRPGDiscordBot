using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IBattleService
    {
        BattleResult Battle(Enemy enemy, Player player);
    }
    public class BattleService : IBattleService
    {
        public BattleResult Battle(Enemy enemy, Player player)
        {
            var result = new BattleResult();
            do
            {
                var atk = player.Atk - enemy.Def;
                if (atk < 0)
                {
                    atk = 0;
                }
                enemy.HP -= atk;
                result.DamageDealt += player.Atk - enemy.Def;
                if (enemy.HP > 0)
                {
                    atk = enemy.Atk - player.Def;
                    if (atk < 0)
                    {
                        atk = 0;
                    }
                    player.CurrentHP -= atk;
                    result.DamageReceived += atk;
                }
                /* awaiting items function to be done, suppose use itemService to calculate how many Items should be used and add calculation on equipment
                var items = player.Bag.Where(x => x.IsEquiped && x.ItemLeft > 0);
                if (items.Count() > 0)
                {
                    //use items
                    if(player.HP <= enemy.Atk && items.Any(x => x.HP > 0))
                    {
                        var item = items.First(x => x.ItemLeft > 0 && x.HP > 0);
                        item.ItemLeft--;
                        player.HP += item.HP;
                        if(result.ItemsUsed.Any(x => x.Key == item))
                        {
                            result.ItemsUsed[item]++;
                        }
                        else
                        {
                            result.ItemsUsed.Add(item, 1);
                        }
                    }

                }*/
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
    }
}
