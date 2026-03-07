namespace ASTRANET_Hidden_Sector.World
{
    public enum LocationType
    {
        Empty,
        Planet,
        Station,
        Enemy,
        Anomaly,
        Resources,
        Hidden
    }

    public class Location
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public LocationType Type { get; set; } = LocationType.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsVisible { get; set; } = false;
        public bool IsExplored { get; set; } = false;

        public Location() { }

        public Location(string name, string description, LocationType type, int x, int y)
        {
            Name = name;
            Description = description;
            Type = type;
            X = x;
            Y = y;
        }
    }
}