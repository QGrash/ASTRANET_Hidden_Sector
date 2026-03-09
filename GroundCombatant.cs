using ASTRANET_Hidden_Sector.Entities;
using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Combat
{
    public class GroundCombatant : CombatantBase
    {
        public int Armor { get; set; }
        public int Evasion { get; set; }
        public int EvasionBonus { get; set; }
        public Weapon Weapon { get; set; }
        public List<Item> Inventory { get; set; }

        public GroundCombatant(string name, int maxHealth, int maxEnergy, int armor, int evasion, Weapon weapon)
            : base(name, maxHealth, maxEnergy)
        {
            Armor = armor;
            Evasion = evasion;
            Weapon = weapon;
            Inventory = new List<Item>();
            EvasionBonus = 0;
        }

        public int TotalEvasion => Evasion + EvasionBonus;

        public override void TakeDamage(int damage, DamageType damageType)
        {
            int mitigatedDamage = (int)(damage * (100.0 / (100 + Armor)));
            base.TakeDamage(mitigatedDamage, damageType);
        }

        public void BeginTurn()
        {
            Energy = MaxEnergy;
            EvasionBonus = 0;
        }

        public bool CanFire()
        {
            return Weapon != null && Energy >= Weapon.EnergyCost;
        }

        public int FireWeapon(GroundCombatant target)
        {
            if (!CanFire()) return 0;

            Energy -= Weapon.EnergyCost;
            int shots = (Weapon.Mode == WeaponMode.Automatic) ? Weapon.ShotsPerTurn : 1;
            int totalDamage = 0;
            System.Random rand = new System.Random();

            for (int i = 0; i < shots; i++)
            {
                int hitChance = 80 + Weapon.Accuracy - target.TotalEvasion;
                if (hitChance < 5) hitChance = 5;
                if (hitChance > 95) hitChance = 95;

                if (rand.Next(100) < hitChance)
                {
                    int damage = Weapon.GetDamage();
                    if (rand.Next(100) < Weapon.CritChance)
                    {
                        damage = (int)(damage * Weapon.CritMultiplier);
                    }
                    target.TakeDamage(damage, Weapon.DamageType);
                    totalDamage += damage;
                }
            }
            return totalDamage;
        }

        public void GainEnergyOnSkip(int amount)
        {
            Energy = System.Math.Min(MaxEnergy, Energy + amount);
        }
    }
}