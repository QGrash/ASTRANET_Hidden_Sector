// Managers/DialogueManager.cs
using ASTRANET.Core;
using ASTRANET.Generators;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Instances;
using ASTRANET.Models.Prototypes;
using ASTRANET.Models.World;
using ASTRANET.Screens;
using ASTRANET.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASTRANET.Managers;

public class DialogueManager
{
    private readonly Dictionary<string, DialogueData> _dialogues = new();
    private readonly string _dialoguesPath = Path.Combine("Data", "Dialogues");

    public DialogueData CurrentDialogue { get; private set; }
    public DialogueNode CurrentNode { get; private set; }

    public void LoadAllDialogues()
    {
        if (!Directory.Exists(_dialoguesPath))
        {
            Directory.CreateDirectory(_dialoguesPath);
        }

        var files = Directory.GetFiles(_dialoguesPath, "*.json");
        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var dialogue = JsonConvert.DeserializeObject<DialogueData>(json);
                if (dialogue != null && !string.IsNullOrEmpty(dialogue.Id))
                {
                    _dialogues[dialogue.Id] = dialogue;
                }
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки
            }
        }

        EnsureTraderGreeting();

        if (_dialogues.Count == 0)
        {
            CreateSampleDialogues();
            files = Directory.GetFiles(_dialoguesPath, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var dialogue = JsonConvert.DeserializeObject<DialogueData>(json);
                    if (dialogue != null && !string.IsNullOrEmpty(dialogue.Id))
                    {
                        _dialogues[dialogue.Id] = dialogue;
                    }
                }
                catch { }
            }
        }
    }

    private void EnsureTraderGreeting()
    {
        if (_dialogues.ContainsKey("trader_greeting")) return;

        var traderDialogue = new DialogueData
        {
            Id = "trader_greeting",
            StartNodeId = "start",
            Nodes = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Id = "start",
                    NpcText = "Приветствую, капитан. Желаете взглянуть на мои товары?",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Да, покажи что у тебя есть.",
                            NextNodeId = "trade",
                            Actions = new List<DialogueAction>
                            {
                                new DialogueAction { Type = DialogueActionType.OpenShop, TargetId = "trader_01" }
                            }
                        },
                        new DialogueChoice
                        {
                            Text = "Нет, спасибо. Мне нужно идти.",
                            NextNodeId = "bye"
                        }
                    }
                },
                ["trade"] = new DialogueNode
                {
                    Id = "trade",
                    NpcText = "Смотри, выбирай.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Вернуться", NextNodeId = "start" }
                    }
                },
                ["bye"] = new DialogueNode
                {
                    Id = "bye",
                    NpcText = "Удачного плавания!",
                    Choices = new List<DialogueChoice>()
                }
            }
        };

        _dialogues["trader_greeting"] = traderDialogue;

        string path = Path.Combine(_dialoguesPath, "trader_greeting.json");
        if (!File.Exists(path))
        {
            var json = JsonConvert.SerializeObject(traderDialogue, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }

    public bool StartDialogue(string dialogueId)
    {
        if (!_dialogues.TryGetValue(dialogueId, out var dialogue))
            return false;

        CurrentDialogue = dialogue;
        if (dialogue.Nodes.TryGetValue(dialogue.StartNodeId, out var startNode))
        {
            CurrentNode = startNode;
            return true;
        }
        return false;
    }

    public bool MakeChoice(int choiceIndex)
    {
        if (CurrentNode == null || choiceIndex < 0 || choiceIndex >= CurrentNode.Choices.Count)
            return false;

        var choice = CurrentNode.Choices[choiceIndex];

        if (!CheckConditions(choice.Conditions))
            return false;

        foreach (var action in choice.Actions)
        {
            ExecuteAction(action);
        }

        if (string.IsNullOrEmpty(choice.NextNodeId))
        {
            EndDialogue();
            return true;
        }

        if (CurrentDialogue.Nodes.TryGetValue(choice.NextNodeId, out var nextNode))
        {
            CurrentNode = nextNode;
            return true;
        }

        EndDialogue();
        return true;
    }

    public void EndDialogue()
    {
        CurrentDialogue = null;
        CurrentNode = null;
        EventBus.Publish(new DialogueEndedEvent());
    }

    private bool CheckConditions(List<DialogueCondition> conditions)
    {
        if (conditions == null || conditions.Count == 0) return true;

        var player = DI.Resolve<Player>();
        var reputationManager = DI.Resolve<ReputationManager>();
        var questManager = DI.Resolve<QuestManager>();

        foreach (var condition in conditions)
        {
            switch (condition.Type)
            {
                case "HasItem":
                    if (!player.Inventory.HasItem(condition.TargetId, condition.Value))
                        return false;
                    break;
                case "HasCredits":
                    if (player.Credits < condition.Value)
                        return false;
                    break;
                case "Reputation":
                    if (reputationManager.GetReputation((FactionId)Enum.Parse(typeof(FactionId), condition.TargetId)) < condition.Value)
                        return false;
                    break;
                case "QuestCompleted":
                    var quest = questManager.ActiveQuests.FirstOrDefault(q => q.PrototypeId == condition.TargetId);
                    if (quest == null || quest.State != QuestState.Completed)
                        return false;
                    break;
                case "QuestActive":
                    var activeQuest = questManager.ActiveQuests.FirstOrDefault(q => q.PrototypeId == condition.TargetId);
                    if (activeQuest == null || activeQuest.State != QuestState.Active)
                        return false;
                    break;
            }
        }
        return true;
    }

    private void ExecuteAction(DialogueAction action)
    {
        var player = DI.Resolve<Player>();
        var itemManager = DI.Resolve<ItemManager>();
        var questManager = DI.Resolve<QuestManager>();
        var reputationManager = DI.Resolve<ReputationManager>();
        var uiManager = DI.Resolve<UIManager>();
        var screenManager = DI.Resolve<ScreenManager>();
        var inputManager = DI.Resolve<InputManager>();

        switch (action.Type)
        {
            case DialogueActionType.AddItem:
                var item = itemManager.CreateInstance(action.TargetId, action.Amount);
                player.Inventory.AddItem(item);
                uiManager.ShowMessage($"Получено: {item.Prototype.Name}");
                break;

            case DialogueActionType.RemoveItem:
                var removeItem = player.Inventory.Items.FirstOrDefault(i => i.PrototypeId == action.TargetId);
                if (removeItem != null)
                    player.Inventory.RemoveItem(removeItem, action.Amount);
                break;

            case DialogueActionType.AddCredits:
                player.Credits += action.Amount;
                uiManager.ShowMessage($"Получено {action.Amount} кредитов");
                break;

            case DialogueActionType.RemoveCredits:
                player.Credits -= action.Amount;
                if (player.Credits < 0) player.Credits = 0;
                uiManager.ShowMessage($"Потрачено {action.Amount} кредитов");
                break;

            case DialogueActionType.ChangeReputation:
                var faction = (FactionId)Enum.Parse(typeof(FactionId), action.TargetId);
                reputationManager.AddReputation(faction, action.Amount);
                break;

            case DialogueActionType.StartQuest:
                questManager.StartQuest(action.TargetId);
                uiManager.ShowMessage($"Новый квест: {questManager.GetPrototype(action.TargetId)?.Name}");
                break;

            case DialogueActionType.CompleteQuest:
                questManager.CompleteQuest(action.TargetId);
                uiManager.ShowMessage("Квест выполнен!");
                break;

            case DialogueActionType.OpenShop:
                var shopManager = DI.Resolve<ShopManager>();
                var shop = shopManager.GetShop(action.TargetId);
                if (shop != null)
                {
                    var shopScreen = new ShopScreen(uiManager, screenManager, inputManager, shop, player);
                    screenManager.PushScreen(shopScreen);
                }
                break;

            case DialogueActionType.ChangeSystemVisibility:
                var worldManager = DI.Resolve<WorldManager>();
                var system = worldManager.Galaxy.Systems.FirstOrDefault(s => s.Id == int.Parse(action.TargetId));
                if (system != null)
                {
                    system.Hidden = false;
                }
                break;

            case DialogueActionType.EnterInterior:
                try
                {
                    var interior = InteriorGenerator.Generate(InteriorType.Station, FactionId.SolarFederation, RandomManager.Next());
                    var interiorScreen = new InteriorScreen(uiManager, screenManager, inputManager, interior, player);
                    screenManager.PushScreen(interiorScreen);
                }
                catch (Exception ex)
                {
                    uiManager.ShowMessage($"Ошибка входа в интерьер: {ex.Message}");
                }
                break;
        }
    }

    private void CreateSampleDialogues()
    {
        var samples = new[]
        {
            new DialogueData
            {
                Id = "federation_officer",
                StartNodeId = "start",
                Nodes = new Dictionary<string, DialogueNode>
                {
                    ["start"] = new DialogueNode
                    {
                        Id = "start",
                        NpcText = "Капитан, рад вас видеть. У нас есть проблема: пираты участили нападения в секторе Альфа. Не могли бы вы помочь?",
                        Choices = new List<DialogueChoice>
                        {
                            new DialogueChoice
                            {
                                Text = "Расскажите подробнее.",
                                NextNodeId = "details"
                            },
                            new DialogueChoice
                            {
                                Text = "Я слишком занят.",
                                NextNodeId = "busy"
                            }
                        }
                    },
                    ["details"] = new DialogueNode
                    {
                        Id = "details",
                        NpcText = "Пиратский корабль «Кровавый рассвет» терроризирует торговцев. Если вы уничтожите его, мы щедро наградим вас.",
                        Choices = new List<DialogueChoice>
                        {
                            new DialogueChoice
                            {
                                Text = "Я согласен. Дайте квест.",
                                NextNodeId = "accept",
                                Actions = new List<DialogueAction>
                                {
                                    new DialogueAction { Type = DialogueActionType.StartQuest, TargetId = "kill_pirates" }
                                }
                            },
                            new DialogueChoice
                            {
                                Text = "Нет, это слишком опасно.",
                                NextNodeId = "bye"
                            }
                        }
                    },
                    ["accept"] = new DialogueNode
                    {
                        Id = "accept",
                        NpcText = "Отлично! Корабль находится в системе Альфа. Удачи, капитан.",
                        Choices = new List<DialogueChoice>
                        {
                            new DialogueChoice { Text = "Буду на связи.", NextNodeId = "bye" }
                        }
                    },
                    ["busy"] = new DialogueNode
                    {
                        Id = "busy",
                        NpcText = "Понимаю. Если передумаете, обращайтесь.",
                        Choices = new List<DialogueChoice>
                        {
                            new DialogueChoice { Text = "До свидания.", NextNodeId = "bye" }
                        }
                    },
                    ["bye"] = new DialogueNode
                    {
                        Id = "bye",
                        NpcText = "Честь имею.",
                        Choices = new List<DialogueChoice>()
                    }
                }
            }
        };

        foreach (var sample in samples)
        {
            var json = JsonConvert.SerializeObject(sample, Formatting.Indented);
            File.WriteAllText(Path.Combine(_dialoguesPath, $"{sample.Id}.json"), json);
        }
    }
}