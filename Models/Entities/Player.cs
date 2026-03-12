// Models/Entities/Player.cs
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Models.Instances;

namespace ASTRANET.Models.Entities;

public class Player
{
    public string Name { get; set; } = "Капитан";

    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Luck { get; set; } = 10;
    public int Charisma { get; set; } = 10;

    public PlayerClass Class { get; set; } = PlayerClass.Soldier;

    public int Health { get; set; } = 100;
    public int MaxHealth { get; set; } = 100;

    public int Hull { get; set; } = 100;
    public int MaxHull { get; set; } = 100;
    public int Shields { get; set; } = 50;
    public int MaxShields { get; set; } = 50;
    public int Energy { get; set; } = 100;
    public int MaxEnergy { get; set; } = 100;
    public int Fuel { get; set; } = 200;
    public int MaxFuel { get; set; } = 200;

    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int NextLevelExp { get; set; } = 100;

    public int Credits { get; set; } = 1000;
    public int TechPoints { get; set; } = 0;

    public Inventory Inventory { get; set; } = new Inventory();
    public List<ShipModule> InstalledModules { get; set; } = new();

    public int X { get; set; }
    public int Y { get; set; }

    public ItemInstance Head { get; set; }
    public ItemInstance Torso { get; set; }
    public ItemInstance Body { get; set; }
    public ItemInstance Hands { get; set; }
    public ItemInstance Legs { get; set; }
    public ItemInstance Weapon { get; set; }
    public ItemInstance Neck { get; set; }
    public List<ItemInstance> Belt { get; set; } = new List<ItemInstance>(3);

    // Флаг смерти
    public bool IsDead => Health <= 0 || Hull <= 0;

    public void ApplyClassBonuses()
    {
        switch (Class)
        {
            case PlayerClass.Soldier:
                Strength += 2;
                Dexterity += 1;
                break;
            case PlayerClass.CombatEngineer:
                Intelligence += 2;
                Dexterity += 1;
                break;
            case PlayerClass.Diplomat:
                Charisma += 2;
                Luck += 1;
                break;
            case PlayerClass.Scout:
                Dexterity += 2;
                Intelligence += 1;
                break;
            case PlayerClass.Engineer:
                Intelligence += 2;
                Strength += 1;
                break;
            case PlayerClass.Medic:
                Intelligence += 2;
                Charisma += 1;
                break;
            case PlayerClass.Quartermaster:
                Charisma += 2;
                Intelligence += 1;
                break;
            case PlayerClass.Cartographer:
                Intelligence += 2;
                Luck += 1;
                break;
        }
    }

    public void RecalculateShipStats()
    {
        MaxEnergy = 100;
        MaxShields = 50;
        MaxHull = 100;

        foreach (var module in InstalledModules)
        {
            var proto = module.Item.Prototype;
            if (proto == null) continue;

            switch (proto.ModuleType)
            {
                case ModuleType.Reactor:
                    MaxEnergy += module.GetCurrentEnergyBonus();
                    break;
                case ModuleType.Shield:
                    MaxShields += module.GetCurrentShieldBonus();
                    break;
            }
        }

        if (Energy > MaxEnergy) Energy = MaxEnergy;
        if (Shields > MaxShields) Shields = MaxShields;
        if (Hull > MaxHull) Hull = MaxHull;
    }

    public int GetTotalDefense()
    {
        int defense = 0;
        if (Head?.Prototype?.Defense != null) defense += Head.Prototype.Defense.Value;
        if (Torso?.Prototype?.Defense != null) defense += Torso.Prototype.Defense.Value;
        if (Body?.Prototype?.Defense != null) defense += Body.Prototype.Defense.Value;
        if (Hands?.Prototype?.Defense != null) defense += Hands.Prototype.Defense.Value;
        if (Legs?.Prototype?.Defense != null) defense += Legs.Prototype.Defense.Value;
        return defense;
    }

    public double GetTotalWeight()
    {
        double weight = Inventory.GetTotalWeight();
        if (Head?.Prototype != null) weight += Head.Prototype.Weight * Head.Quantity;
        if (Torso?.Prototype != null) weight += Torso.Prototype.Weight * Torso.Quantity;
        if (Body?.Prototype != null) weight += Body.Prototype.Weight * Body.Quantity;
        if (Hands?.Prototype != null) weight += Hands.Prototype.Weight * Hands.Quantity;
        if (Legs?.Prototype != null) weight += Legs.Prototype.Weight * Legs.Quantity;
        if (Weapon?.Prototype != null) weight += Weapon.Prototype.Weight * Weapon.Quantity;
        if (Neck?.Prototype != null) weight += Neck.Prototype.Weight * Neck.Quantity;
        foreach (var item in Belt)
            if (item?.Prototype != null)
                weight += item.Prototype.Weight * item.Quantity;
        return weight;
    }

    public int GetMaxWeight() => 20 + Strength * 5;
    public bool IsOverEncumbered() => GetTotalWeight() > GetMaxWeight();

    public void AddExperience(int amount)
    {
        Experience += amount;
        while (Experience >= NextLevelExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        Experience -= NextLevelExp;
        NextLevelExp = (int)(NextLevelExp * 1.5);
        MaxHealth += 10;
        Health = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        int defense = GetTotalDefense();
        int actualDamage = damage - defense;
        if (actualDamage < 0) actualDamage = 0;
        Health -= actualDamage;
        if (Health < 0) Health = 0;
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth) Health = MaxHealth;
    }

    public bool ConsumeFuel(int amount)
    {
        if (Fuel < amount) return false;
        Fuel -= amount;
        return true;
    }

    public void Refuel(int amount)
    {
        Fuel += amount;
        if (Fuel > MaxFuel) Fuel = MaxFuel;
    }

    public void TakeHullDamage(int damage)
    {
        if (Shields > 0)
        {
            int shieldDamage = System.Math.Min(Shields, damage);
            Shields -= shieldDamage;
            damage -= shieldDamage;
        }
        if (damage > 0)
        {
            Hull -= damage;
            if (Hull < 0) Hull = 0;
        }
    }

    public void RestoreEnergy(int amount)
    {
        Energy += amount;
        if (Energy > MaxEnergy) Energy = MaxEnergy;
    }

    public void Regenerate()
    {
        Energy = MaxEnergy;
        Shields += 5;
        if (Shields > MaxShields) Shields = MaxShields;
    }

    public bool ActivateSOS()
    {
        if (Energy < 20) return false;
        Energy -= 20;
        EventBus.Publish(new SOSBeaconActivatedEvent());
        return true;
    }
}