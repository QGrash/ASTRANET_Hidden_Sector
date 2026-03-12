// Models/World/Galaxy.cs
using System.Collections.Generic;

namespace ASTRANET.Models.World;

public class Galaxy
{
    public List<StarSystem> Systems { get; set; } = new();
    public int Seed { get; set; }
}