using ASTRANET_Hidden_Sector.Entities.Quest;
using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Data
{
    public class SaveData
    {
        public string PlayerName { get; set; } = "";
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int Credits { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Luck { get; set; }
        public int Charisma { get; set; }
        public string BackgroundId { get; set; } = "";

        public List<SavedItem> Inventory { get; set; } = new();
        public List<SavedQuest> Quests { get; set; } = new();
        public Dictionary<string, int> Reputation { get; set; } = new();

        public int WorldSeed { get; set; }
        public string CurrentSector { get; set; } = "";
        public string CurrentSystem { get; set; } = "";
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public bool InLocalMap { get; set; }
    }

    public class SavedItem
    {
        public string Id { get; set; } = "";
        public int Count { get; set; } = 1;
    }
}