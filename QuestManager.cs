using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public static class QuestManager
    {
        private static Dictionary<string, Quest> allQuests = new();
        private static List<Quest> activeQuests = new();
        private static List<Quest> completedQuests = new();

        public static void LoadAll()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Quests");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                CreateDefaultQuests(path);
            }

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                var json = File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<QuestData>(json);
                if (data != null && !string.IsNullOrEmpty(data.Id))
                {
                    var quest = new Quest(data);
                    allQuests[data.Id] = quest;
                }
            }
        }

        public static void StartQuest(string questId)
        {
            if (allQuests.ContainsKey(questId))
            {
                var quest = allQuests[questId];
                quest.Start();
                activeQuests.Add(quest);
            }
        }

        public static void UpdateProgress(ObjectiveType type, string target, int amount = 1)
        {
            foreach (var quest in activeQuests.ToArray()) // ToArray чтобы избежать изменения коллекции во время итерации
            {
                quest.UpdateProgress(type, target, amount);
                if (quest.State == QuestState.Completed)
                {
                    activeQuests.Remove(quest);
                    completedQuests.Add(quest);
                    // Автоматически выдаём награды
                    foreach (var reward in quest.Rewards)
                    {
                        reward.Grant();
                    }
                }
            }
        }

        public static List<Quest> GetActiveQuests() => activeQuests;
        public static List<Quest> GetCompletedQuests() => completedQuests;
        public static Quest GetQuest(string id) => allQuests.ContainsKey(id) ? allQuests[id] : null;

        public static List<SavedQuest> ExportState()
        {
            var saved = new List<SavedQuest>();
            foreach (var quest in allQuests.Values)
            {
                var sq = new SavedQuest
                {
                    Id = quest.Id,
                    State = quest.State
                };
                foreach (var obj in quest.Objectives)
                {
                    sq.ObjectiveProgress[obj.Id] = obj.CurrentAmount;
                }
                saved.Add(sq);
            }
            return saved;
        }

        public static void ImportState(List<SavedQuest> saved)
        {
            foreach (var sq in saved)
            {
                if (allQuests.ContainsKey(sq.Id))
                {
                    var quest = allQuests[sq.Id];
                    quest.State = sq.State;
                    foreach (var obj in quest.Objectives)
                    {
                        if (sq.ObjectiveProgress.ContainsKey(obj.Id))
                            obj.CurrentAmount = sq.ObjectiveProgress[obj.Id];
                        else
                            obj.CurrentAmount = 0;
                        // Пересчитываем IsCompleted
                        obj.IsCompleted = obj.CurrentAmount >= obj.RequiredAmount;
                    }
                    // Добавляем обратно в списки активных/выполненных, если нужно
                    if (quest.State == QuestState.Active && !activeQuests.Contains(quest))
                        activeQuests.Add(quest);
                    else if (quest.State == QuestState.Completed && !completedQuests.Contains(quest))
                        completedQuests.Add(quest);
                }
            }
        }

        private static void CreateDefaultQuests(string path)
        {
            // Пример простого квеста
            var sampleQuest = new QuestData
            {
                Id = "find_fuel",
                Name = "Поиск топлива",
                Description = "На станции закончилось топливо. Найдите 3 канистры с топливом в астероидном поле.",
                Type = QuestType.Side,
                Objectives = new List<QuestObjectiveData>
                {
                    new QuestObjectiveData
                    {
                        Id = "collect_fuel",
                        Description = "Собрать топливо: 0/3",
                        Type = ObjectiveType.CollectItem,
                        Target = "fuel",
                        RequiredAmount = 3
                    }
                },
                Rewards = new List<QuestRewardData>
                {
                    new QuestRewardData { Type = RewardType.Credits, Amount = 500 },
                    new QuestRewardData { Type = RewardType.Reputation, Target = "traders", Amount = 10 }
                }
            };

            var json = JsonConvert.SerializeObject(sampleQuest, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "find_fuel.json"), json);
        }
    }
}