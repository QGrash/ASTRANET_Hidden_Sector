// Models/Entities/ShipModule.cs
using ASTRANET.Models.Instances;

namespace ASTRANET.Models.Entities;

public class ShipModule
{
    public ItemInstance Item { get; set; }
    public int Level { get; set; } = 1;
    public int TechPointsSpent { get; set; } = 0;

    // Расчёт улучшенных характеристик на основе прототипа и уровня
    public int GetCurrentEnergyBonus()
    {
        if (Item.Prototype?.EnergyBonus == null) return 0;
        return (int)(Item.Prototype.EnergyBonus.Value * (1 + (Level - 1) * 0.1));
    }

    public int GetCurrentShieldBonus()
    {
        if (Item.Prototype?.ShieldBonus == null) return 0;
        return (int)(Item.Prototype.ShieldBonus.Value * (1 + (Level - 1) * 0.1));
    }

    public int GetCurrentHullBonus()
    {
        if (Item.Prototype?.HullBonus == null) return 0;
        return (int)(Item.Prototype.HullBonus.Value * (1 + (Level - 1) * 0.1));
    }

    public int GetCurrentEnginePower()
    {
        if (Item.Prototype?.EnginePower == null) return 0;
        return (int)(Item.Prototype.EnginePower.Value * (1 + (Level - 1) * 0.1));
    }

    public int GetCurrentEvasionBonus()
    {
        if (Item.Prototype?.EvasionBonus == null) return 0;
        return (int)(Item.Prototype.EvasionBonus.Value * (1 + (Level - 1) * 0.1));
    }

    public int GetCurrentCargoBonus()
    {
        if (Item.Prototype?.CargoBonus == null) return 0;
        return (int)(Item.Prototype.CargoBonus.Value * (1 + (Level - 1) * 0.1));
    }

    // Для оружия
    public int GetCurrentDamageMin()
    {
        if (Item.Prototype?.DamageMin == null) return 0;
        return (int)(Item.Prototype.DamageMin.Value * (1 + (Level - 1) * 0.1));
    }

    public int GetCurrentDamageMax()
    {
        if (Item.Prototype?.DamageMax == null) return 0;
        return (int)(Item.Prototype.DamageMax.Value * (1 + (Level - 1) * 0.1));
    }

    public int GetCurrentEnergyCost()
    {
        if (Item.Prototype?.EnergyCost == null) return 0;
        // Улучшение может снижать энергопотребление
        return (int)(Item.Prototype.EnergyCost.Value * (1 - (Level - 1) * 0.05));
    }

    public int GetNextUpgradeCost()
    {
        // Стоимость следующего улучшения в TechPoints
        return Level * 10;
    }
}