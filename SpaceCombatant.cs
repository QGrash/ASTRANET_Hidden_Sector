using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Combat
{
    public class SpaceCombatant : CombatantBase
    {
        public Shield Shield { get; set; }
        public int Evasion { get; set; }
        public int EvasionBonus { get; set; }
        public List<Weapon> Weapons { get; set; }

        public SpaceCombatant(string name, int maxHealth, int maxEnergy, Shield shield, int evasion, List<Weapon> weapons)
            : base(name, maxHealth, maxEnergy)
        {
            Shield = shield;
            Evasion = evasion;
            Weapons = weapons ?? new List<Weapon>();
            EvasionBonus = 0;
        }

        public int TotalEvasion => Evasion + EvasionBonus;

        public override void TakeDamage(int damage, DamageType damageType)
        {
            if (Shield != null && Shield.IsActive)
            {
                damage = Shield.AbsorbDamage(damage);
            }
            base.TakeDamage(damage, damageType);
        }

        public void BeginTurn()
        {
            Shield?.Regenerate();
            Energy = MaxEnergy;
            EvasionBonus = 0;
        }

        public bool CanFire(Weapon weapon)
        {
            return Energy >= weapon.EnergyCost;
        }

        public int FireWeapon(Weapon weapon, SpaceCombatant target)
        {
            if (!CanFire(weapon)) return 0;

            Energy -= weapon.EnergyCost;
            int shots = (weapon.Mode == WeaponMode.Automatic) ? weapon.ShotsPerTurn : 1;
            int totalDamage = 0;
            System.Random rand = new System.Random();

            for (int i = 0; i < shots; i++)
            {
                int hitChance = 80 + weapon.Accuracy - target.TotalEvasion;
                if (hitChance < 5) hitChance = 5;
                if (hitChance > 95) hitChance = 95;

                if (rand.Next(100) < hitChance)
                {
                    int damage = weapon.GetDamage();
                    if (rand.Next(100) < weapon.CritChance)
                    {
                        damage = (int)(damage * weapon.CritMultiplier);
                    }
                    target.TakeDamage(damage, weapon.DamageType);
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