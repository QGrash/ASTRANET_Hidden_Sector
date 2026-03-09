using System;

namespace ASTRANET_Hidden_Sector.Combat
{
    public abstract class CombatantBase : ICombatant
    {
        public string Name { get; protected set; }
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; protected set; }

        public bool IsAlive => Health > 0;

        protected CombatantBase(string name, int maxHealth, int maxEnergy)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            MaxEnergy = maxEnergy;
            Energy = maxEnergy;
        }

        public virtual void TakeDamage(int damage, DamageType damageType)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        public virtual void Die() { }
    }
}