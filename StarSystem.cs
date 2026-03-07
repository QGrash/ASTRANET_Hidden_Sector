using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Data;

namespace ASTRANET_Hidden_Sector.World
{
    public class StarSystem
    {
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public SystemType Type { get; set; } = SystemType.Habitable;
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<StarSystem> ConnectedSystems { get; set; } = new List<StarSystem>();
        public bool IsExplored { get; set; } = false;
        public bool IsHidden { get; set; } = false;
        public LocalMap? LocalMap { get; set; } = null;

        public StarSystem() { }

        public StarSystem(string name, int x, int y, SystemType type)
        {
            Name = name;
            X = x;
            Y = y;
            Type = type;
        }

        // Конструктор для восстановления из сохранения
        public StarSystem(SavedStarSystem saved, List<StarSystem> allSystemsInSector)
        {
            Name = saved.Name;
            X = saved.X;
            Y = saved.Y;
            Type = saved.Type;
            IsExplored = saved.IsExplored;
            IsHidden = saved.IsHidden;

            // Восстанавливаем связи по именам (передаём список всех систем сектора)
            foreach (var connName in saved.ConnectedSystemNames)
            {
                var conn = allSystemsInSector.Find(s => s.Name == connName);
                if (conn != null)
                    ConnectedSystems.Add(conn);
            }

            // Восстанавливаем локальную карту, если она сохранена
            if (saved.LocalMap != null)
            {
                LocalMap = new LocalMap(saved.LocalMap);
            }
        }
    }
}