// Models/Combat/SpaceShip.cs
using System.Collections.Generic;
using ASTRANET.Models.Prototypes;

namespace ASTRANET.Models.Combat;

public class SpaceShip
{
    public string Id { get; set; }
    public string Name { get; set; }
    public FactionId Faction { get; set; } // Фракция корабля
    public int Hull { get; set; }
    public int MaxHull { get; set; }
    public SpaceShield Shields { get; set; }
    public int Energy { get; set; }
    public int MaxEnergy { get; set; }
    public int Accuracy { get; set; } = 0;
    public int Evasion { get; set; } = 0;
    public List<SpaceWeapon> Weapons { get; set; } = new();
    public bool IsAlive => Hull > 0;

    public SpaceShip(string id, string name, FactionId faction, int hull, int shield, int energy)
    {
        Id = id;
        Name = name;
        Faction = faction;
        MaxHull = hull;
        Hull = hull;
        Shields = new SpaceShield(shield);
        MaxEnergy = energy;
        Energy = energy;
    }

    public void TakeDamage(int damage, DamageType damageType)
    {
        int remaining = Shields.AbsorbDamage(damage);
        if (remaining > 0)
        {
            Hull -= remaining;
            if (Hull < 0) Hull = 0;
        }
    }

    public void BeginTurn()
    {
        Energy = MaxEnergy;
        Shields.Recharge();
    }

    public bool CanFire(SpaceWeapon weapon)
    {
        return weapon.IsReady && Energy >= weapon.EnergyCost;
    }
}