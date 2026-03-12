// Models/World/InteriorCell.cs
using System;

namespace ASTRANET.Models.World;

public class InteriorCell
{
    public char Tile { get; set; } = ' ';
    public bool Walkable { get; set; } = false;
    public bool Visible { get; set; } = false;
    public bool Explored { get; set; } = false;
    public int EntityId { get; set; } = -1;
    public ConsoleColor Color { get; set; } = ConsoleColor.Gray;

    public InteriorCell(char tile, bool walkable, ConsoleColor color = ConsoleColor.Gray)
    {
        Tile = tile;
        Walkable = walkable;
        Color = color;
    }
}