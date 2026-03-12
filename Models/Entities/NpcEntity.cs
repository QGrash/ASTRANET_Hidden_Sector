// Models/Entities/NpcEntity.cs
namespace ASTRANET.Models.Entities;

public class NpcEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public NpcType Type { get; set; }
    public FactionId Faction { get; set; }
    public string DialogueId { get; set; } // ID диалога для этого NPC
    public int X { get; set; }
    public int Y { get; set; }

    // Боевые характеристики (для врагов)
    public int Health { get; set; } = 50;
    public int MaxHealth { get; set; } = 50;
    public int Shield { get; set; } = 0;
    public int Attack { get; set; } = 10;
    public int Defense { get; set; } = 5;

    public bool IsHostile => Type == NpcType.Enemy;
}

public enum NpcType
{
    Friendly,
    Neutral,
    Enemy
}