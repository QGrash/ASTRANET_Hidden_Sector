// Models/Combat/SpaceWeapon.cs
using ASTRANET.Models.Prototypes;

namespace ASTRANET.Models.Combat;

public class SpaceWeapon
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DamageType DamageType { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public int Accuracy { get; set; }
    public int EnergyCost { get; set; }
    public int ShotsPerTurn { get; set; } = 1;
    public double CritChance { get; set; } = 0.05;
    public double CritMultiplier { get; set; } = 1.5;
    public bool IsReady { get; set; } = true;

    public SpaceWeapon(ItemPrototype proto)
    {
        Id = proto.Id;
        Name = proto.Name;
        DamageType = proto.DamageType ?? DamageType.Laser;
        MinDamage = proto.DamageMin ?? 5;
        MaxDamage = proto.DamageMax ?? 10;
        Accuracy = proto.Accuracy ?? 0;
        EnergyCost = proto.EnergyCost ?? 5;
        ShotsPerTurn = proto.ShotsPerTurn ?? 1;
        CritChance = proto.CritChance ?? 0.05;
        CritMultiplier = proto.CritMultiplier ?? 1.5;
    }
}