// Data/SaveData.cs
using ASTRANET.Models.Instances;
using System;
using System.Collections.Generic;

namespace ASTRANET.Data;

[Serializable]
public class SaveData
{
    public string Version { get; set; } = "1.0";
    public DateTime Timestamp { get; set; }
    public string PlayerName { get; set; } // имя персонажа
    public int GalaxySeed { get; set; }
    public PlayerData Player { get; set; }
    public WorldData World { get; set; }
    public QuestsData Quests { get; set; }
    public ReputationData Reputation { get; set; }
}

[Serializable]
public class PlayerData
{
    public string Name { get; set; } // имя
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    public int Luck { get; set; }
    public int Charisma { get; set; }
    public PlayerClass Class { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Hull { get; set; }
    public int MaxHull { get; set; }
    public int Shields { get; set; }
    public int MaxShields { get; set; }
    public int Energy { get; set; }
    public int MaxEnergy { get; set; }
    public int Fuel { get; set; }
    public int MaxFuel { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int NextLevelExp { get; set; }
    public int Credits { get; set; }
    public int TechPoints { get; set; }
    public List<ItemData> Inventory { get; set; } = new();
    public string Head { get; set; }
    public string Torso { get; set; }
    public string Body { get; set; }
    public string Hands { get; set; }
    public string Legs { get; set; }
    public string Weapon { get; set; }
    public string Neck { get; set; }
    public List<string> Belt { get; set; } = new();
    public List<ModuleData> InstalledModules { get; set; } = new();
}

[Serializable]
public class ItemData
{
    public string PrototypeId { get; set; }
    public int Quantity { get; set; } = 1;
    public int CurrentDurability { get; set; } = -1;
}

[Serializable]
public class ModuleData
{
    public ItemData Item { get; set; }
    public int Level { get; set; } = 1;
    public int TechPointsSpent { get; set; } = 0;
}

[Serializable]
public class WorldData
{
    public List<int> VisitedSystems { get; set; } = new();
    public List<int> RevealedHiddenSystems { get; set; } = new();
}

[Serializable]
public class QuestsData
{
    public List<QuestInstanceData> ActiveQuests { get; set; } = new();
    public List<string> CompletedQuests { get; set; } = new();
}

[Serializable]
public class QuestInstanceData
{
    public string PrototypeId { get; set; }
    public QuestState State { get; set; }
    public List<int> ObjectiveProgress { get; set; } = new();
}

[Serializable]
public class ReputationData
{
    public Dictionary<FactionId, int> Values { get; set; } = new();
}