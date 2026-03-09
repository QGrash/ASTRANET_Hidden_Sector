using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ASTRANET_Hidden_Sector.Data;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public static class InteriorLoader
    {
        private static Dictionary<string, InteriorTemplate> templates = new();

        public static void LoadAll()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Interiors");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                CreateDefaultTemplate(path);
            }

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                try
                {
                    string json = File.ReadAllText(file, Encoding.UTF8);
                    var template = JsonConvert.DeserializeObject<InteriorTemplate>(json);
                    if (template != null && !string.IsNullOrEmpty(template.Id))
                    {
                        templates[template.Id] = template;
                    }
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                }
            }
        }

        public static InteriorTemplate GetTemplate(string id)
        {
            return templates.ContainsKey(id) ? templates[id] : null;
        }

        private static void CreateDefaultTemplate(string path)
        {
            var defaultTemplate = new InteriorTemplate
            {
                Id = "test_corridor",
                Name = "Тестовый коридор",
                Width = 10,
                Height = 8,
                Tiles = new List<string>
                {
                    "##########",
                    "#........#",
                    "#..C....B#",
                    "#...X....#",
                    "#........#",
                    "#........#",
                    "#........#",
                    "##########"
                },
                Entities = new List<TemplateEntity>
                {
                    new TemplateEntity
                    {
                        Type = "npc",
                        Subtype = "trader",
                        Name = "Торговец",
                        X = 3,
                        Y = 2,
                        DialogueId = "trader_01",
                        Color = "green",
                        Symbol = "T"
                    },
                    new TemplateEntity
                    {
                        Type = "container",
                        Name = "Ящик",
                        X = 4,
                        Y = 3,
                        Loot = new List<string> { "medkit", "fuel" },
                        Locked = false
                    },
                    new TemplateEntity
                    {
                        Type = "console",
                        Name = "Терминал",
                        X = 2,
                        Y = 2,
                        Description = "Мигающий экран.",
                        Action = "open_door"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(defaultTemplate, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "test_corridor.json"), json, Encoding.UTF8);
        }
    }
}