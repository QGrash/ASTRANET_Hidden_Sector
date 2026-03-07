namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueActionData
    {
        public string Type { get; set; } = "";
        public string Target { get; set; } = "";
        public int Value { get; set; }
    }
    public enum ActionType
    {
        // ... существующие
        StartQuest,
        CompleteQuest,
        UpdateQuestObjective,
        SetQuestStage
    }
}