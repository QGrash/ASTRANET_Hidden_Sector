namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public class QuestObjective
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public ObjectiveType Type { get; set; }
        public string Target { get; set; }
        public int RequiredAmount { get; set; }
        public int CurrentAmount { get; set; }
        public bool IsCompleted { get; set; }

        public QuestObjective(QuestObjectiveData data)
        {
            Id = data.Id;
            Description = data.Description;
            Type = data.Type;
            Target = data.Target;
            RequiredAmount = data.RequiredAmount;
            CurrentAmount = 0;
            IsCompleted = false;
        }
    }
}