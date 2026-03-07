using ASTRANET_Hidden_Sector.World;
using System;
using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Data
{
    [Serializable]
    public class SavedWorld
    {
        public int Seed { get; set; }
        public List<SavedSector> Sectors { get; set; } = new();
        public string CurrentSectorId { get; set; } = "";
        public string CurrentSystemId { get; set; } = "";
        public int LocalPlayerX { get; set; }
        public int LocalPlayerY { get; set; }
        public string CurrentScreen { get; set; } = "Galaxy";
    }

    [Serializable]
    public class SavedSector
    {
        public string Name { get; set; } = "";
        public bool IsLocked { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<string> ConnectedSectorIds { get; set; } = new();
        public List<SavedStarSystem> Systems { get; set; } = new();
    }

    [Serializable]
    public class SavedStarSystem
    {
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public SystemType Type { get; set; }
        public bool IsExplored { get; set; }
        public bool IsHidden { get; set; }
        public List<string> ConnectedSystemNames { get; set; } = new();
        public SavedLocalMap LocalMap { get; set; }
    }

    [Serializable]
    public class SavedLocalMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<SavedMapCell> Cells { get; set; } = new();
    }

    [Serializable]
    public class SavedMapCell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public SavedEntity Entity { get; set; }
        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }
        public char Symbol { get; set; }
        public ConsoleColor Color { get; set; }
    }

    [Serializable]
    public class SavedEntity
    {
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public char Symbol { get; set; }
        public ConsoleColor Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsStatic { get; set; }
        public string LocationType { get; set; } = "";
        public string DialogueId { get; set; } = "";
        public int Health { get; set; }
    }
}