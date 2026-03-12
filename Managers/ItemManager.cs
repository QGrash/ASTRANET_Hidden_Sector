// Managers/ItemManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASTRANET.Models.Prototypes;
using ASTRANET.Models.Instances;
using Newtonsoft.Json;

namespace ASTRANET.Managers;

public class ItemManager
{
    private readonly Dictionary<string, ItemPrototype> _prototypes = new();
    private readonly string _itemsPath = Path.Combine("Data", "Items");

    public void LoadAllPrototypes()
    {
        if (!Directory.Exists(_itemsPath))
        {
            Directory.CreateDirectory(_itemsPath);
        }

        var files = Directory.GetFiles(_itemsPath, "*.json");
        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                // Проверяем, является ли JSON массивом
                if (json.TrimStart().StartsWith("["))
                {
                    var array = JsonConvert.DeserializeObject<List<ItemPrototype>>(json);
                    if (array != null)
                    {
                        foreach (var item in array)
                        {
                            if (item != null && !string.IsNullOrEmpty(item.Id))
                                _prototypes[item.Id] = item;
                        }
                    }
                }
                else
                {
                    var prototype = JsonConvert.DeserializeObject<ItemPrototype>(json);
                    if (prototype != null && !string.IsNullOrEmpty(prototype.Id))
                    {
                        _prototypes[prototype.Id] = prototype;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки {file}: {ex.Message}");
            }
        }

        EnsureCoreItems();
    }

    private void EnsureCoreItems()
    {
        if (!_prototypes.ContainsKey("laser_pistol"))
        {
            _prototypes["laser_pistol"] = new ItemPrototype
            {
                Id = "laser_pistol",
                Name = "Лазерный пистолет",
                Description = "Стандартное лёгкое лазерное оружие.",
                Weight = 1.2,
                BasePrice = 200,
                ItemType = ItemType.Equipment,
                Slot = EquipmentSlot.Weapon,
                DamageMin = 5,
                DamageMax = 8,
                Accuracy = 5,
                EnergyCost = 5,
                ShotsPerTurn = 1,
                CritChance = 0.05,
                CritMultiplier = 1.5,
                DamageType = DamageType.Laser,
                WeaponClass = WeaponClass.Light
            };
        }

        if (!_prototypes.ContainsKey("medkit_basic"))
        {
            _prototypes["medkit_basic"] = new ItemPrototype
            {
                Id = "medkit_basic",
                Name = "Медицинский набор",
                Description = "Восстанавливает 30 HP.",
                Weight = 0.1,
                BasePrice = 50,
                ItemType = ItemType.Consumable,
                Effects = new Dictionary<string, int> { { "heal", 30 } }
            };
        }

        if (!_prototypes.ContainsKey("armor_light"))
        {
            _prototypes["armor_light"] = new ItemPrototype
            {
                Id = "armor_light",
                Name = "Лёгкая броня",
                Description = "Обеспечивает базовую защиту.",
                Weight = 3.0,
                BasePrice = 150,
                ItemType = ItemType.Equipment,
                Slot = EquipmentSlot.Torso,
                Defense = 10
            };
        }

        if (!_prototypes.ContainsKey("weapon_laser"))
        {
            _prototypes["weapon_laser"] = new ItemPrototype
            {
                Id = "weapon_laser",
                Name = "Лазерная пушка",
                Description = "Стандартное лазерное орудие.",
                Weight = 2.0,
                BasePrice = 400,
                ItemType = ItemType.Module,
                ModuleType = ModuleType.Weapon,
                DamageMin = 10,
                DamageMax = 15,
                Accuracy = 5,
                EnergyCost = 10,
                ShotsPerTurn = 1,
                CritChance = 0.05,
                CritMultiplier = 1.5,
                DamageType = DamageType.Laser
            };
        }
    }

    public ItemPrototype GetPrototype(string id)
    {
        _prototypes.TryGetValue(id, out var proto);
        return proto;
    }

    public ItemInstance CreateInstance(string id, int quantity = 1)
    {
        var proto = GetPrototype(id);
        if (proto == null)
            throw new ArgumentException($"Прототип {id} не найден");
        return new ItemInstance(id, quantity) { Prototype = proto };
    }

    public List<ItemPrototype> GetAllPrototypes() => _prototypes.Values.ToList();
}