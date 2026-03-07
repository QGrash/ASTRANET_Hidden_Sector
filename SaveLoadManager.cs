using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ASTRANET_Hidden_Sector.Data
{
    [Serializable]
    public class CombinedSaveData
    {
        public SaveData Player { get; set; }
        public SavedWorld World { get; set; }
        public DateTime SaveTime { get; set; }
        public string GameVersion { get; set; }
    }

    public class SaveMetadata
    {
        public string FileName { get; set; }
        public string PlayerName { get; set; }
        public int Level { get; set; }
        public string Sector { get; set; }
        public string System { get; set; }
        public DateTime SaveTime { get; set; }
        public string GameVersion { get; set; }
    }

    public static class SaveLoadManager
    {
        private static string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

        static SaveLoadManager()
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
        }

        public static void SaveGame(string fileName, SaveData playerData, SavedWorld worldData)
        {
            var combined = new CombinedSaveData
            {
                Player = playerData,
                World = worldData,
                SaveTime = DateTime.Now,
                GameVersion = "1.0"
            };

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(combined, settings);
            string fullPath = Path.Combine(savePath, fileName + ".json");
            File.WriteAllText(fullPath, json);
        }

        public static CombinedSaveData LoadGame(string fileName)
        {
            string fullPath = Path.Combine(savePath, fileName + ".json");
            if (!File.Exists(fullPath))
                return null;
            string json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<CombinedSaveData>(json);
        }

        public static void DeleteSave(string fileName)
        {
            string fullPath = Path.Combine(savePath, fileName + ".json");
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public static List<SaveMetadata> GetAllSaves()
        {
            var files = Directory.GetFiles(savePath, "*.json");
            var result = new List<SaveMetadata>();

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var combined = JsonConvert.DeserializeObject<CombinedSaveData>(json);
                    if (combined != null)
                    {
                        result.Add(new SaveMetadata
                        {
                            FileName = Path.GetFileNameWithoutExtension(file),
                            PlayerName = combined.Player?.PlayerName ?? "Unknown",
                            Level = combined.Player?.Level ?? 1,
                            Sector = combined.World?.CurrentSectorId ?? "Unknown",
                            System = combined.World?.CurrentSystemId ?? "Unknown",
                            SaveTime = combined.SaveTime,
                            GameVersion = combined.GameVersion
                        });
                    }
                }
                catch { /* игнорируем повреждённые файлы */ }
            }

            return result.OrderByDescending(m => m.SaveTime).ToList();
        }
    }
}