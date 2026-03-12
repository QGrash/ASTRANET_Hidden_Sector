// Enums.cs
namespace ASTRANET;

public enum TerrainType
{
    Empty,
    Planet,
    Station,
    Anomaly,
    EnemyShip,
    NeutralShip,
    FriendlyShip,
    Wreck,
    Asteroid,
    Nebula
}

public enum SystemType
{
    Habitable,
    Desert,
    Hostile,
    Anomaly,
    Hidden
}

public enum ItemType
{
    Consumable,
    Resource,
    Equipment,
    Module,
    Quest
}

public enum EquipmentSlot
{
    Head,
    Torso,
    Body,
    Hands,
    Legs,
    Weapon,
    Neck,
    Belt
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

public enum FireMode
{
    Single,
    Auto
}

public enum WeaponClass
{
    Light,
    Medium,
    Heavy
}

public enum ModuleType
{
    Reactor,
    Engine,
    Shield,
    Weapon,
    Utility,
    LifeSupport,
    Cargo
}

public enum FactionId
{
    SolarFederation,
    PyroThetaEmpire,
    AquaTarrissRepublic,
    BrogNomads,
    GreenCrossFoundation,
    MercArmory,
    CultOfVoid,
    TarrshakAqua,
    MegaTransen,
    CrimsonVoyagers,
    VoidDragons
}

public enum ReputationLevel
{
    Hostile,
    Negative,
    Neutral,
    Friendly,
    Ally
}

public enum DialogueActionType
{
    AddItem,
    RemoveItem,
    ChangeReputation,
    AddCredits,
    RemoveCredits,
    OpenShop,
    StartQuest,
    CompleteQuest,
    SetQuestProgress,
    ChangeSystemVisibility,
    EnterInterior
}

public enum EventType
{
    PlayerMoved,
    PlayerHealthChanged,
    PlayerCreditsChanged,
    ShipShieldsChanged,
    ShipHullChanged,
    ShipEnergyChanged,
    ReputationChanged,
    QuestProgress,
    EnemyKilled,
    ItemUsed,
    WindowResized,
    SOSBeaconActivated,
    SystemSelected,
    LocalMapEntered,
    CombatStarted,
    CombatEnded,
    DialogueStarted,
    DialogueEnded,
    GameSaved,
    GameLoaded
}

public enum PlayerClass
{
    Soldier,
    CombatEngineer,
    Diplomat,
    Scout,
    Engineer,
    Medic,
    Quartermaster,
    Cartographer
}

public enum NpcType
{
    Friendly,
    Neutral,
    Enemy
}

public enum NpcBehavior
{
    Stationary,
    Patrol
}

public enum InteriorType
{
    Shuttle,
    Station,
    PlanetaryBase
}