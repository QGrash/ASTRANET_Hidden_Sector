// Managers/QuestManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Data;
using ASTRANET.Models.Instances;
using ASTRANET.Models.Prototypes;
using Newtonsoft.Json;

namespace ASTRANET.Managers;

public class QuestManager
{
    private readonly Dictionary<string, QuestPrototype> _prototypes = new();
    private readonly List<QuestInstance> _activeQuests = new();
    private readonly string _questsPath = Path.Combine("Data", "Quests");

    public IReadOnlyList<QuestInstance> ActiveQuests => _activeQuests.AsReadOnly();

    public void LoadAllPrototypes()
    {
        if (!Directory.Exists(_questsPath))
        {
            Directory.CreateDirectory(_questsPath);
            CreateSampleQuests();
        }

        var files = Directory.GetFiles(_questsPath, "*.json");
        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var proto = JsonConvert.DeserializeObject<QuestPrototype>(json);
                if (proto != null && !string.IsNullOrEmpty(proto.Id))
                {
                    _prototypes[proto.Id] = proto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки квеста {file}: {ex.Message}");
            }
        }
    }

    public QuestPrototype GetPrototype(string id)
    {
        _prototypes.TryGetValue(id, out var proto);
        return proto;
    }

    public bool StartQuest(string id)
    {
        var proto = GetPrototype(id);
        if (proto == null) return false;

        if (_activeQuests.Any(q => q.PrototypeId == id)) return false;

        var quest = new QuestInstance(id)
        {
            Prototype = proto,
            State = QuestState.Active,
            ObjectiveProgress = proto.Objectives.Select(o => 0).ToList()
        };
        _activeQuests.Add(quest);
        EventBus.Publish(new QuestProgressEvent { QuestId = id, Progress = 0 });
        return true;
    }

    public void UpdateProgress(string id, string targetId, int amount = 1)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.PrototypeId == id);
        if (quest == null) return;

        for (int i = 0; i < quest.Prototype.Objectives.Count; i++)
        {
            var obj = quest.Prototype.Objectives[i];
            if (obj.TargetId == targetId)
            {
                quest.ObjectiveProgress[i] += amount;
                if (quest.ObjectiveProgress[i] >= obj.Amount)
                {
                    quest.ObjectiveProgress[i] = obj.Amount;
                }

                bool allComplete = true;
                for (int j = 0; j < quest.Prototype.Objectives.Count; j++)
                {
                    if (quest.ObjectiveProgress[j] < quest.Prototype.Objectives[j].Amount)
                    {
                        allComplete = false;
                        break;
                    }
                }

                if (allComplete)
                {
                    quest.State = QuestState.Completed;
                    EventBus.Publish(new QuestProgressEvent { QuestId = id, Progress = 100, Completed = true });
                }
                else
                {
                    int totalProgress = quest.ObjectiveProgress.Sum() * 100 / quest.Prototype.Objectives.Sum(o => o.Amount);
                    EventBus.Publish(new QuestProgressEvent { QuestId = id, Progress = totalProgress });
                }
                break;
            }
        }
    }

    public bool CompleteQuest(string id)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.PrototypeId == id && q.State == QuestState.Completed);
        if (quest == null) return false;

        var player = DI.Resolve<Models.Entities.Player>();
        if (quest.Prototype.Rewards.Credits > 0)
            player.Credits += quest.Prototype.Rewards.Credits;
        if (quest.Prototype.Rewards.Experience > 0)
            player.AddExperience(quest.Prototype.Rewards.Experience);
        foreach (var rep in quest.Prototype.Rewards.Reputation)
        {
            var repManager = DI.Resolve<ReputationManager>();
            repManager.AddReputation(rep.Key, rep.Value);
        }
        var itemManager = DI.Resolve<ItemManager>();
        foreach (var itemId in quest.Prototype.Rewards.Items)
        {
            var item = itemManager.CreateInstance(itemId, 1);
            player.Inventory.AddItem(item);
        }

        try
        {
            var techManager = DI.Resolve<TechManager>();
            techManager.AddTechPoints(10);
        }
        catch { }

        quest.State = QuestState.Finished;
        _activeQuests.Remove(quest);
        return true;
    }

    // Методы для загрузки/очистки
    public void LoadQuest(QuestInstanceData data)
    {
        var proto = GetPrototype(data.PrototypeId);
        if (proto == null) return;
        var quest = new QuestInstance(data.PrototypeId)
        {
            Prototype = proto,
            State = data.State,
            ObjectiveProgress = new List<int>(data.ObjectiveProgress)
        };
        _activeQuests.Add(quest);
    }

    public void ClearQuests()
    {
        _activeQuests.Clear();
    }

    private void CreateSampleQuests()
    {
        var samples = new[]
        {
            new QuestPrototype
            {
                Id = "kill_pirates",
                Name = "Зачистка пиратов",
                Description = "Уничтожьте пиратский корабль в системе Альфа.",
                Objectives = new List<QuestObjective>
                {
                    new QuestObjective { Type = "Kill", TargetId = "pirate_ship", Amount = 1, Description = "Уничтожить пиратский корабль" }
                },
                Rewards = new QuestReward
                {
                    Credits = 500,
                    Experience = 100,
                    Items = new List<string> { "medkit_basic" }
                }
            }
        };

        foreach (var sample in samples)
        {
            var json = JsonConvert.SerializeObject(sample, Formatting.Indented);
            File.WriteAllText(Path.Combine(_questsPath, $"{sample.Id}.json"), json);
        }
    }
}