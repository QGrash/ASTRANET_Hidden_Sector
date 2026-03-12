// Models/Combat/SpaceShield.cs
using System;

namespace ASTRANET.Models.Combat;

public class SpaceShield
{
    public int MaxShield { get; set; }
    public int CurrentShield { get; set; }
    public int RechargeRate { get; set; }

    public SpaceShield(int max, int recharge = 5)
    {
        MaxShield = max;
        CurrentShield = max;
        RechargeRate = recharge;
    }

    public void Recharge()
    {
        CurrentShield += RechargeRate;
        if (CurrentShield > MaxShield) CurrentShield = MaxShield;
    }

    public int AbsorbDamage(int damage)
    {
        int absorbed = Math.Min(CurrentShield, damage);
        CurrentShield -= absorbed;
        return damage - absorbed;
    }
}