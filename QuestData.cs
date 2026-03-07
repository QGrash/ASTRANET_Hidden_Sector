using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public class QuestData
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public QuestType Type { get; set; } = QuestType.Main; // Main, Side, Hidden
        public List<QuestObjectiveData> Objectives { get; set; } = new();
        public List<QuestRewardData> Rewards { get; set; } = new();
        public string NextQuestId { get; set; } = ""; // для цепочки квестов
        public Dictionary<string, string> CustomData { get; set; } = new(); // для дополнительных параметров
    }

    public enum QuestType
    {
        Main,
        Side,
        Hidden
    }
}