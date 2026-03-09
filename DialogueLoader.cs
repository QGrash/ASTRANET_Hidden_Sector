using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public static class DialogueLoader
    {
        private static Dictionary<string, DialogueData> dialogues = new();

        public static void LoadAll()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Dialogues");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                CreateDefaultDialogue(path);
            }

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                try
                {
                    // Чтение в UTF-8
                    string json = File.ReadAllText(file, Encoding.UTF8);
                    var dialogue = JsonConvert.DeserializeObject<DialogueData>(json);
                    if (dialogue != null && !string.IsNullOrEmpty(dialogue.Id))
                    {
                        dialogues[dialogue.Id] = dialogue;
                    }
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                }
            }
        }

        public static DialogueData GetDialogue(string id)
        {
            return dialogues.ContainsKey(id) ? dialogues[id] : null;
        }

        public static Dictionary<string, DialogueData> GetAll()
        {
            return dialogues;
        }

        private static void CreateDefaultDialogue(string path)
        {
            var defaultDialogue = new DialogueData
            {
                Id = "trader_01",
                NpcName = "Торговец",
                StartNodeId = "greeting",
                Nodes = new Dictionary<string, DialogueNodeData>
                {
                    ["greeting"] = new DialogueNodeData
                    {
                        Id = "greeting",
                        NpcText = "Приветствую, путник! Чем могу помочь?",
                        Choices = new List<DialogueChoiceData>
                        {
                            new DialogueChoiceData
                            {
                                ChoiceText = "Что у вас есть?",
                                NextNodeId = "trade",
                                Conditions = new(),
                                Actions = new()
                            },
                            new DialogueChoiceData
                            {
                                ChoiceText = "Пока",
                                NextNodeId = "",
                                Conditions = new(),
                                Actions = new()
                            }
                        }
                    },
                    ["trade"] = new DialogueNodeData
                    {
                        Id = "trade",
                        NpcText = "У меня есть товары. Что хотите?",
                        Choices = new List<DialogueChoiceData>
                        {
                            new DialogueChoiceData
                            {
                                ChoiceText = "Посмотреть",
                                NextNodeId = "",
                                Actions = new List<DialogueActionData>
                                {
                                    new DialogueActionData { Type = "openShop", Target = "trader_01", Value = 0 }
                                }
                            },
                            new DialogueChoiceData
                            {
                                ChoiceText = "Вернуться",
                                NextNodeId = "greeting"
                            }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(defaultDialogue, Formatting.Indented);
            // Запись в UTF-8
            File.WriteAllText(Path.Combine(path, "trader_01.json"), json, Encoding.UTF8);
        }
    }
}