using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.World
{
    public class Sector
    {
        public string Name { get; set; } = "";
        public bool IsLocked { get; set; } = false;
        public List<StarSystem> Systems { get; set; } = new List<StarSystem>();
        public List<Sector> ConnectedSectors { get; set; } = new List<Sector>();
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;

        public Sector() { }

        public Sector(string name, bool isLocked = false, int x = 0, int y = 0)
        {
            Name = name;
            IsLocked = isLocked;
            X = x;
            Y = y;
        }

        public Sector(Data.SavedSector saved, Dictionary<string, Sector> sectorDict)
        {
            Name = saved.Name;
            IsLocked = saved.IsLocked;
            X = saved.X;
            Y = saved.Y;
        }
    }
}