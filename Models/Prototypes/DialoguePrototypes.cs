// Models/Prototypes/DialoguePrototypes.cs
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ASTRANET.Models.Prototypes;

public class DialogueData
{
    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("StartNodeId")]
    public string StartNodeId { get; set; }

    [JsonProperty("Nodes")]
    public Dictionary<string, DialogueNode> Nodes { get; set; } = new();
}

public class DialogueNode
{
    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("NpcText")]
    public string NpcText { get; set; }

    [JsonProperty("Choices")]
    public List<DialogueChoice> Choices { get; set; } = new();
}

public class DialogueChoice
{
    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("NextNodeId")]
    public string NextNodeId { get; set; }

    [JsonProperty("Conditions")]
    public List<DialogueCondition> Conditions { get; set; } = new();

    [JsonProperty("Actions")]
    public List<DialogueAction> Actions { get; set; } = new();
}

public class DialogueCondition
{
    [JsonProperty("Type")]
    public string Type { get; set; } // "HasItem", "HasCredits", "Reputation", "QuestCompleted", "QuestActive"

    [JsonProperty("TargetId")]
    public string TargetId { get; set; } // ID предмета, фракции, квеста

    [JsonProperty("Value")]
    public int Value { get; set; } // количество, порог репутации и т.д.
}

public class DialogueAction
{
    [JsonProperty("Type")]
    public DialogueActionType Type { get; set; }

    [JsonProperty("TargetId")]
    public string TargetId { get; set; }

    [JsonProperty("Amount")]
    public int Amount { get; set; }
}