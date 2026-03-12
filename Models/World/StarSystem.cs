// Models/World/StarSystem.cs
using System.Collections.Generic;

namespace ASTRANET.Models.World;

public class StarSystem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public SystemType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public List<int> ConnectedSystems { get; set; } = new();
    public bool Visited { get; set; } = false;
    public bool Hidden { get; set; } = false;

    public StarSystem(int id, string name, SystemType type, int x, int y)
    {
        Id = id;
        Name = name;
        Type = type;
        X = x;
        Y = y;
    }
}