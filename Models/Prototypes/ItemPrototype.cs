// Models/Prototypes/ItemPrototype.cs
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ASTRANET.Models.Prototypes;

public class ItemPrototype
{
    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Weight")]
    public double Weight { get; set; }

    [JsonProperty("BasePrice")]
    public int BasePrice { get; set; }

    [JsonProperty("ItemType")]
    public ItemType ItemType { get; set; }

    [JsonProperty("Effects")]
    public Dictionary<string, int> Effects { get; set; } = new();

    // Для оружия
    [JsonProperty("DamageMin")]
    public int? DamageMin { get; set; }

    [JsonProperty("DamageMax")]
    public int? DamageMax { get; set; }

    [JsonProperty("Accuracy")]
    public int? Accuracy { get; set; }

    [JsonProperty("EnergyCost")]
    public int? EnergyCost { get; set; }

    [JsonProperty("ShotsPerTurn")]
    public int? ShotsPerTurn { get; set; }

    [JsonProperty("CritChance")]
    public double? CritChance { get; set; }

    [JsonProperty("CritMultiplier")]
    public double? CritMultiplier { get; set; }

    [JsonProperty("DamageType")]
    public DamageType? DamageType { get; set; }

    [JsonProperty("WeaponClass")]
    public WeaponClass? WeaponClass { get; set; }

    // Для брони
    [JsonProperty("Defense")]
    public int? Defense { get; set; }

    [JsonProperty("Slot")]
    public EquipmentSlot? Slot { get; set; }

    // Для модулей корабля
    [JsonProperty("ModuleType")]
    public ModuleType? ModuleType { get; set; }

    [JsonProperty("EnergyBonus")]
    public int? EnergyBonus { get; set; }

    [JsonProperty("ShieldBonus")]
    public int? ShieldBonus { get; set; }

    [JsonProperty("HullBonus")]
    public int? HullBonus { get; set; }

    [JsonProperty("EnginePower")]
    public int? EnginePower { get; set; }

    [JsonProperty("EvasionBonus")]
    public int? EvasionBonus { get; set; }

    [JsonProperty("CargoBonus")]
    public int? CargoBonus { get; set; }
}