// Models/Entities/SpaceNpc.cs
namespace ASTRANET.Models.Entities;

public class SpaceNpc
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ASTRANET.NpcType Type { get; set; }
    public FactionId Faction { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; } = 50;
    public int MaxHealth { get; set; } = 50;
    public int Attack { get; set; } = 8;
    public int Defense { get; set; } = 3;
    public string DialogueId { get; set; }
    public bool IsPatrol { get; set; } = false;
    public int PatrolTargetX { get; set; }
    public int PatrolTargetY { get; set; }
    public bool IsAlive => Health > 0;

    public SpaceNpc() { }
}