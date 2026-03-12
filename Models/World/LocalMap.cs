// Models/World/LocalMap.cs
using System;
using System.Collections.Generic;
using ASTRANET.Models.Entities;

namespace ASTRANET.Models.World;

public class LocalMap
{
    public const int Width = 40;
    public const int Height = 20;
    public const int VisibilityRadius = 5;

    private readonly Cell[,] _cells = new Cell[Height, Width];
    public List<SpaceNpc> Npcs { get; set; } = new();

    public LocalMap()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _cells[y, x] = new Cell();
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return null;
        return _cells[y, x];
    }

    public void SetCell(int x, int y, Cell cell) => _cells[y, x] = cell;

    public void SetVisible(int playerX, int playerY)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _cells[y, x].Visible = false;

        for (int dy = -VisibilityRadius; dy <= VisibilityRadius; dy++)
        {
            for (int dx = -VisibilityRadius; dx <= VisibilityRadius; dx++)
            {
                int nx = playerX + dx;
                int ny = playerY + dy;
                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                {
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    if (dist <= VisibilityRadius + 0.5)
                    {
                        _cells[ny, nx].Visible = true;
                    }
                }
            }
        }
    }

    public SpaceNpc GetNpcAt(int x, int y)
    {
        return Npcs.Find(n => n.X == x && n.Y == y && n.IsAlive);
    }

    public void AddNpc(SpaceNpc npc)
    {
        if (npc != null && !Npcs.Contains(npc))
            Npcs.Add(npc);
    }
}