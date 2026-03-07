using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ASTRANET_Hidden_Sector.Entities.Trade
{
    public static class TradeGoodsLoader
    {
        private static Dictionary<string, TradeGood> goods = new();

        public static void LoadAll()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TradeGoods");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                CreateDefaultGoods(path);
            }

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var data = JsonConvert.DeserializeObject<TradeGoodData>(json);
                    if (data != null && !string.IsNullOrEmpty(data.Id))
                    {
                        goods[data.Id] = new TradeGood(data);
                    }
                }
                catch (Exception ex)
                {
                    // логирование ошибки
                }
            }
        }

        public static TradeGood GetGood(string id)
        {
            return goods.ContainsKey(id) ? goods[id] : null;
        }

        public static List<TradeGood> GetAllGoods()
        {
            return new List<TradeGood>(goods.Values);
        }

        private static void CreateDefaultGoods(string path)
        {
            // Несколько примеров товаров
            var examples = new[]
            {
                new TradeGoodData
                {
                    Id = "medkit_basic",
                    Name = "Медицинский набор",
                    Description = "Стандартный армейский медпакет, восстанавливает 30 HP.",
                    Category = "Consumable",
                    Weight = 0.1f,
                    BasePrice = 50,
                    HealAmount = 30
                },
                new TradeGoodData
                {
                    Id = "stim_basic",
                    Name = "Стимулятор",
                    Description = "Временно повышает рефлексы. (В бою не реализовано)",
                    Category = "Consumable",
                    Weight = 0.1f,
                    BasePrice = 40,
                    HealAmount = 10
                },
                new TradeGoodData
                {
                    Id = "fuel_cell",
                    Name = "Топливная ячейка",
                    Description = "Стандартный блок топлива для прыжков.",
                    Category = "Resource",
                    Weight = 10f,
                    BasePrice = 50
                },
                new TradeGoodData
                {
                    Id = "metal_alloy",
                    Name = "Сплав металла",
                    Description = "Используется для ремонта и крафта.",
                    Category = "Resource",
                    Weight = 5f,
                    BasePrice = 30
                },
                new TradeGoodData
                {
                    Id = "laser_pistol",
                    Name = "Лазерный пистолет",
                    Description = "Легкое лазерное оружие для самообороны.",
                    Category = "Weapon",
                    Weight = 1.2f,
                    BasePrice = 200
                }
            };

            foreach (var example in examples)
            {
                var json = JsonConvert.SerializeObject(example, Formatting.Indented);
                File.WriteAllText(Path.Combine(path, example.Id + ".json"), json);
            }
        }
    }
}