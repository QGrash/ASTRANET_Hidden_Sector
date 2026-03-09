using ASTRANET_Hidden_Sector.World;
using System;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public class InteriorCell
    {
        public FloorType Floor { get; set; } = FloorType.Metal;
        public WallType NorthWall { get; set; } = WallType.None;
        public WallType SouthWall { get; set; } = WallType.None;
        public WallType WestWall { get; set; } = WallType.None;
        public WallType EastWall { get; set; } = WallType.None;
        public MapEntity? Entity { get; set; }
        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }

        public char FloorSymbol => Floor switch
        {
            FloorType.Metal => '░',
            FloorType.Wood => '▒',
            FloorType.Carpet => '▓',
            FloorType.Grate => '╬',
            _ => '·'
        };

        public ConsoleColor FloorColor => Floor switch
        {
            FloorType.Metal => ConsoleColor.DarkGray,
            FloorType.Wood => ConsoleColor.DarkYellow,
            FloorType.Carpet => ConsoleColor.DarkRed,
            FloorType.Grate => ConsoleColor.Gray,
            _ => ConsoleColor.DarkGray
        };
    }

    public enum FloorType
    {
        Metal,
        Wood,
        Carpet,
        Grate,
        Empty
    }

    public enum WallType
    {
        None,
        Solid,
        Door,
        OpenDoor,
        Window
    }
}