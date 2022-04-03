using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Models
{
    public class Player
    {
        /// <summary>
        /// 玩家ID, 也是Discord ID
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        /// 防止玩家去其他ser係唔正確的地圖上刷怪
        /// </summary>
        public ulong ServerId { get; set; }
        /// <summary>
        /// 玩家有幾多錢
        /// </summary>
        public int Coin { get; set; }
        /// <summary>
        /// 基礎HP點數，可用來計算最高HP數值
        /// </summary>
        public int HP { get; set; }
        /// <summary>
        /// 當前HP
        /// </summary>
        public int CurrentHP { get; set; }
        /// <summary>
        /// 當前經驗值
        /// </summary>
        public int Exp { get; set; }
        /// <summary>
        /// 玩家等級
        /// </summary>
        public int Lv { get; set; }
        /// <summary>
        /// 基礎攻擊點數
        /// </summary>
        public int Atk { get; set; }
        /// <summary>
        /// 基礎防禦點數
        /// </summary>
        public int Def { get; set; }
        /// <summary>
        /// 技能點數，可以用來點HP，攻擊力或者防禦
        /// </summary>
        public int SkillPoint { get; set; }
        /// <summary>
        /// 特殊頭銜
        /// </summary>
        public List<string> Title { get; set; }
        /// <summary>
        /// 背包
        /// </summary>
        public List<ItemEquip> Bag { get; set; }
        /// <summary>
        /// 玩家地圖上位置
        /// </summary>
        public Coordinate Coordinate { get; set; }
        /// <summary>
        /// 防止Spam
        /// </summary>
        public DateTime NextCommand { get; set; }
        /// <summary>
        /// 米亞代幣，通關完畢後會自動統計
        /// </summary>
        public int MyaCoin { get; set; }
        /// <summary>
        /// 殺死怪物數量
        /// </summary>
        public int KilledEnemies { get; set; }
        /// <summary>
        /// 統計對Boss造成幾多傷害
        /// </summary>
        public int BossDamage { get; set; }
        /// <summary>
        /// 總共歷代打死幾多怪
        /// </summary>
        public long TotalKilledEnemies { get; set; }
        /// <summary>
        /// 角色最高傷害記錄
        /// </summary>
        public int HighestDmg { get; set; }
        /// <summary>
        /// 角色最高防禦記錄
        /// </summary>
        public int HighestDef { get; set; }
        /// <summary>
        /// 角色最高血量記錄
        /// </summary>
        public int HighestHP { get; set; }
        /// <summary>
        /// 玩家當前層數，用來判定數據使用
        /// </summary>
        public int CurrentStage { get; set; }
    }

    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
