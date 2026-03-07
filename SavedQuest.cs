using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public class SavedQuest
    {
        public string Id { get; set; } = "";
        public QuestState State { get; set; }
        public Dictionary<string, int> ObjectiveProgress { get; set; } = new(); // Key = objective.Id, Value = currentAmount
    }
}