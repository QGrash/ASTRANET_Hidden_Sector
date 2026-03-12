// Models/Instances/QuestInstance.cs
using System.Collections.Generic;
using ASTRANET.Models.Prototypes;

namespace ASTRANET.Models.Instances;

public class QuestInstance
{
    public string PrototypeId { get; set; }
    public QuestState State { get; set; } = QuestState.Inactive;
    public List<int> ObjectiveProgress { get; set; } = new(); // прогресс по каждой цели

    [Newtonsoft.Json.JsonIgnore]
    public QuestPrototype Prototype { get; set; }

    public QuestInstance() { }

    public QuestInstance(string prototypeId)
    {
        PrototypeId = prototypeId;
    }
}

public enum QuestState
{
    Inactive,   // ещё не взято
    Active,     // взято, в процессе
    Completed,  // выполнено, но награда ещё не получена
    Finished    // награда получена, квест завершён
}