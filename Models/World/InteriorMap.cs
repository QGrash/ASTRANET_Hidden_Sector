// Models/World/InteriorMap.cs
using System.Collections.Generic;
using ASTRANET.Models.Entities;

namespace ASTRANET.Models.World;

public class InteriorMap
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    private InteriorCell[,] _cells;
    public List<(int x, int y)> Exits { get; set; } = new();
    public List<InteriorNpc> Npcs { get; set; } = new();
    public List<(int x, int y)> Containers { get; set; } = new(); // координаты контейнеров

    public InteriorMap(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new InteriorCell[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                _cells[y, x] = new InteriorCell(' ', false);
    }

    public InteriorCell GetCell(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return null;
        return _cells[y, x];
    }

    public void SetCell(int x, int y, InteriorCell cell)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            _cells[y, x] = cell;
    }

    // Получить NPC в клетке
    public InteriorNpc GetNpcAt(int x, int y)
    {
        return Npcs.Find(n => n.X == x && n.Y == y);
    }
}