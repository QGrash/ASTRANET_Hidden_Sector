// Core/Events.cs
using System;
using System.Collections.Generic;

namespace ASTRANET.Core;

public abstract class GameEvent : EventArgs { }

public class PlayerMovedEvent : GameEvent
{
    public int OldX { get; set; }
    public int OldY { get; set; }
    public int NewX { get; set; }
    public int NewY { get; set; }
}

public class PlayerHealthChangedEvent : GameEvent
{
    public int OldHealth { get; set; }
    public int NewHealth { get; set; }
    public int MaxHealth { get; set; }
}

public class PlayerCreditsChangedEvent : GameEvent
{
    public int OldCredits { get; set; }
    public int NewCredits { get; set; }
}

public class ShipShieldsChangedEvent : GameEvent
{
    public int OldShields { get; set; }
    public int NewShields { get; set; }
    public int MaxShields { get; set; }
}

public class ShipHullChangedEvent : GameEvent
{
    public int OldHull { get; set; }
    public int NewHull { get; set; }
    public int MaxHull { get; set; }
}

public class ShipEnergyChangedEvent : GameEvent
{
    public int OldEnergy { get; set; }
    public int NewEnergy { get; set; }
    public int MaxEnergy { get; set; }
}

public class ReputationChangedEvent : GameEvent
{
    public FactionId Faction { get; set; }
    public int OldValue { get; set; }
    public int NewValue { get; set; }
}

public class QuestProgressEvent : GameEvent
{
    public string QuestId { get; set; }
    public int Progress { get; set; }
    public bool Completed { get; set; }
}

public class EnemyKilledEvent : GameEvent
{
    public string EnemyId { get; set; }
    public FactionId EnemyFaction { get; set; }
    public int RewardCredits { get; set; }
}

public class QuestCompletedEvent : GameEvent
{
    public string QuestId { get; set; }
    public FactionId? Faction { get; set; }
}

public class ItemUsedEvent : GameEvent
{
    public string ItemId { get; set; }
    public int Quantity { get; set; }
}

public class WindowResizedEvent : GameEvent
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class SOSBeaconActivatedEvent : GameEvent { }

public class SystemSelectedEvent : GameEvent
{
    public int SystemIndex { get; set; }
    public string SystemName { get; set; }
}

public class LocalMapEnteredEvent : GameEvent
{
    public string SystemName { get; set; }
}

public class CombatStartedEvent : GameEvent
{
    public bool IsSpaceCombat { get; set; }
    public List<string> EnemyIds { get; set; } = new();
}

public class CombatEndedEvent : GameEvent
{
    public bool PlayerVictory { get; set; }
}

public class DialogueStartedEvent : GameEvent
{
    public string NpcName { get; set; }
    public string DialogueId { get; set; }
}

public class DialogueEndedEvent : GameEvent { }

public class GameSavedEvent : GameEvent
{
    public string FileName { get; set; }
}

public class GameLoadedEvent : GameEvent
{
    public string FileName { get; set; }
}