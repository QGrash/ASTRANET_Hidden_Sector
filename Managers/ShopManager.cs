// Managers/ShopManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using ASTRANET.Models.Prototypes;
using Newtonsoft.Json;

namespace ASTRANET.Managers;

public class ShopManager
{
    private readonly Dictionary<string, ShopPrototype> _shops = new();
    private readonly string _shopsPath = Path.Combine("Data", "Shops");

    public void LoadAllShops()
    {
        if (!Directory.Exists(_shopsPath))
        {
            Directory.CreateDirectory(_shopsPath);
            CreateSampleShops();
        }

        var files = Directory.GetFiles(_shopsPath, "*.json");
        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var shop = JsonConvert.DeserializeObject<ShopPrototype>(json);
                if (shop != null && !string.IsNullOrEmpty(shop.Id))
                {
                    _shops[shop.Id] = shop;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки магазина {file}: {ex.Message}");
            }
        }
    }

    public ShopPrototype GetShop(string id)
    {
        _shops.TryGetValue(id, out var shop);
        return shop;
    }

    private void CreateSampleShops()
    {
        var samples = new[]
        {
            new ShopPrototype
            {
                Id = "trader_01",
                Name = "Лавка торговца",
                Items = new List<ShopItem>
                {
                    new ShopItem { ItemId = "medkit_basic", PriceModifier = 1.2, Quantity = 5 },
                    new ShopItem { ItemId = "laser_pistol", PriceModifier = 1.0, Quantity = 1 },
                    new ShopItem { ItemId = "armor_light", PriceModifier = 1.1, Quantity = 2 },
                    new ShopItem { ItemId = "fuel_cell", PriceModifier = 0.9, Quantity = -1 }
                }
            },
            new ShopPrototype
            {
                Id = "pyrotech_shop",
                Name = "Имперский арсенал",
                Items = new List<ShopItem>
                {
                    new ShopItem { ItemId = "plasma_rifle", PriceModifier = 1.5, Quantity = 1 },
                    new ShopItem { ItemId = "shield_std", PriceModifier = 1.2, Quantity = 2 },
                    new ShopItem { ItemId = "energy_cell", PriceModifier = 1.0, Quantity = -1 }
                }
            }
        };

        foreach (var sample in samples)
        {
            var json = JsonConvert.SerializeObject(sample, Formatting.Indented);
            File.WriteAllText(Path.Combine(_shopsPath, $"{sample.Id}.json"), json);
        }
    }
}