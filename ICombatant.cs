namespace ASTRANET_Hidden_Sector.Combat
{
    public interface ICombatant
    {
        string Name { get; }
        int Health { get; set; }
        int MaxHealth { get; }
        int Energy { get; set; }
        int MaxEnergy { get; }
        bool IsAlive { get; }
        void TakeDamage(int damage, DamageType damageType);
        void Die();
    }

    public enum DamageType
    {
        Laser,
        Plasma,
        Ion,
        Proton,
        Kinetic,
        Explosive
    }
}