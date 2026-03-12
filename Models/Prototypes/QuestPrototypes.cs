// Models/Prototypes/QuestPrototypes.cs
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ASTRANET.Models.Prototypes;

public class QuestPrototype
{
    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Objectives")]
    public List<QuestObjective> Objectives { get; set; } = new();

    [JsonProperty("Rewards")]
    public QuestReward Rewards { get; set; } = new();

    [JsonProperty("NextQuestId")]
    public string NextQuestId { get; set; } // для цепочки квестов
}

public class QuestObjective
{
    [JsonProperty("Type")]
    public string Type { get; set; } // "Kill", "Collect", "Visit", "Talk"

    [JsonProperty("TargetId")]
    public string TargetId { get; set; } // ID врага, предмета, системы, NPC

    [JsonProperty("Amount")]
    public int Amount { get; set; } = 1;

    [JsonProperty("Description")]
    public string Description { get; set; }
}

public class QuestReward
{
    [JsonProperty("Credits")]
    public int Credits { get; set; }

    [JsonProperty("Experience")]
    public int Experience { get; set; }

    [JsonProperty("Reputation")]
    public Dictionary<FactionId, int> Reputation { get; set; } = new();

    [JsonProperty("Items")]
    public List<string> Items { get; set; } = new(); // ID предметов
}