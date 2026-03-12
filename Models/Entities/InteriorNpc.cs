// Models/Entities/InteriorNpc.cs
using ASTRANET.Models.Instances;

namespace ASTRANET.Models.Entities;

public class InteriorNpc
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ASTRANET.NpcType Type { get; set; }
    public FactionId Faction { get; set; }
    public NpcBehavior Behavior { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int PatrolTargetX { get; set; }
    public int PatrolTargetY { get; set; }
    public string DialogueId { get; set; } // ID диалога для дружественных/нейтральных

    public int Health { get; set; } = 30;
    public int MaxHealth { get; set; } = 30;
    public int Attack { get; set; } = 5;
    public int Defense { get; set; } = 2;

    public List<ItemInstance> Inventory { get; set; } = new();

    public bool IsAlive => Health > 0;
    public bool IsDead => !IsAlive;
}