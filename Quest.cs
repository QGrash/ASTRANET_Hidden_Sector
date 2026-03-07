using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public class Quest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuestType Type { get; set; }
        public List<QuestObjective> Objectives { get; set; } = new();
        public List<QuestReward> Rewards { get; set; } = new();
        public string NextQuestId { get; set; }
        public QuestState State { get; set; } = QuestState.NotStarted;

        public Quest(QuestData data)
        {
            Id = data.Id;
            Name = data.Name;
            Description = data.Description;
            Type = data.Type;
            NextQuestId = data.NextQuestId;

            foreach (var objData in data.Objectives)
            {
                Objectives.Add(new QuestObjective(objData));
            }

            foreach (var rewData in data.Rewards)
            {
                Rewards.Add(new QuestReward(rewData));
            }
        }

        public void Start()
        {
            State = QuestState.Active;
            foreach (var obj in Objectives)
            {
                obj.CurrentAmount = 0;
            }
        }

        public void UpdateProgress(ObjectiveType type, string target, int amount = 1)
        {
            if (State != QuestState.Active) return;

            foreach (var obj in Objectives)
            {
                if (obj.Type == type && obj.Target == target && !obj.IsCompleted)
                {
                    obj.CurrentAmount += amount;
                    if (obj.CurrentAmount >= obj.RequiredAmount)
                    {
                        obj.IsCompleted = true;
                    }
                }
            }

            CheckCompletion();
        }

        private void CheckCompletion()
        {
            foreach (var obj in Objectives)
            {
                if (!obj.IsCompleted) return;
            }
            State = QuestState.Completed;
        }
    }

    public enum QuestState
    {
        NotStarted,
        Active,
        Completed,
        Failed
    }
}