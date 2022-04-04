﻿using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface ISpawnerService
    {
        Enemy Spawn(params MapItem[] tile);
    }
    public class SpawnerService : ISpawnerService
    {
        private readonly Enemies _enemy;
        public SpawnerService(Enemies enemies)
        {
            _enemy = enemies;
        }
        public Enemy Spawn(params MapItem[] tile)
        {
            if(tile.Length == 0)
            {
                return null;
            }
            Random rnd = new Random();
            var selectedGene = tile.Length > 1? tile[rnd.Next(tile.Length-1)]:tile[0];
            var probability = rnd.NextDouble();
            if(probability < 0.3)
            {
                //no spawn
                return null;
            }
            switch (selectedGene)
            {
                case MapItem.Wall:
                    var enemies = _enemy.Where(x => x.Element == Element.Earth).ToList();
                    if(enemies.Count == 0)
                    {
                        return null;
                    }
                    var enemySelected = rnd.Next(enemies.Count() - 1);
                    return enemies[enemySelected];
                case MapItem.Water:
                    enemies = _enemy.Where(x => x.Element == Element.Water).ToList();
                    if (enemies.Count == 0)
                    {
                        return null;
                    }
                    enemySelected = rnd.Next(enemies.Count() - 1);
                    return enemies[enemySelected];
                case MapItem.Lava:
                    enemies = _enemy.Where(x => x.Element == Element.Fire).ToList();
                    if (enemies.Count == 0)
                    {
                        return null;
                    }
                    enemySelected = rnd.Next(enemies.Count() - 1);
                    return enemies[enemySelected];
                case MapItem.Land:
                    enemies = _enemy.Where(x => x.Element == Element.Wind).ToList();
                    if (enemies.Count == 0)
                    {
                        return null;
                    }
                    enemySelected = rnd.Next(enemies.Count() - 1);
                    return enemies[enemySelected];
            }
            return null;
        }
    }
}