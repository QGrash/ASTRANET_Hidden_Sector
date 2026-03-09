using System;

namespace ASTRANET_Hidden_Sector.Combat
{
    public class Weapon
    {
        public string Name { get; set; }
        public DamageType DamageType { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int Accuracy { get; set; }
        public int EnergyCost { get; set; }
        public int ShotsPerTurn { get; set; }
        public int CritChance { get; set; }
        public float CritMultiplier { get; set; }
        public WeaponClass Class { get; set; }
        public WeaponMode Mode { get; set; }

        public Weapon(string name, DamageType damageType, int minDamage, int maxDamage, int accuracy, int energyCost, int shotsPerTurn, int critChance, float critMultiplier, WeaponClass weaponClass, WeaponMode mode)
        {
            Name = name;
            DamageType = damageType;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            Accuracy = accuracy;
            EnergyCost = energyCost;
            ShotsPerTurn = shotsPerTurn;
            CritChance = critChance;
            CritMultiplier = critMultiplier;
            Class = weaponClass;
            Mode = mode;
        }

        public int GetDamage()
        {
            return new Random().Next(MinDamage, MaxDamage + 1);
        }
    }

    public enum WeaponClass
    {
        Light,
        Medium,
        Heavy
    }

    public enum WeaponMode
    {
        Single,
        Automatic
    }
}