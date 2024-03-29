﻿using MyaDiscordBot.Models.Books;

namespace MyaDiscordBot.Models
{
    public class Player
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Player name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 防止玩家去其他ser係唔正確的地圖上刷怪
        /// </summary>
        public ulong ServerId { get; set; }
        /// <summary>
        /// Discord個人ID
        /// </summary>
        public ulong DiscordId { get; set; }
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
        /// 資源背包
        /// </summary>
        public List<HoldedResource> ResourceBag { get; set; }
        /// <summary>
        /// 防止Spam
        /// </summary>
        public DateTime NextCommand { get; set; }
        /// <summary>
        /// 上次Send Command的時間，用來計算線下獎勵
        /// </summary>
        public DateTime LastCommand { get; set; }
        /// <summary>
        /// 統計對Boss造成幾多傷害
        /// </summary>
        public int BossDamage { get; set; }
        /// <summary>
        /// Next rob time
        /// </summary>
        public DateTime NextRob { get; set; }
        /// <summary>
        /// Robbed Shield
        /// </summary>
        public DateTime RobShield { get; set; }
        /// <summary>
        /// Time limit for gambling
        /// </summary>
        public DateTime GamblingDelay { get; set; }
        /// <summary>
        /// Gambling times
        /// </summary>
        public int GamblingTimes { get; set; }
        /// <summary>
        /// Pets
        /// </summary>
        public List<Pet> Pets { get; set; }
        /// <summary>
        /// Books
        /// </summary>
        public List<Book> Books { get; set; }
        /// <summary>
        /// Married user, pretty useless
        /// </summary>
        public ulong MarriedUser { get; set; } = 0;
        /// <summary>
        /// The time that two players married
        /// </summary>
        public DateTime MarriedTime { get; set; }
    }

    public enum BattleType
    {
        Default,
        Trial
    }

}
