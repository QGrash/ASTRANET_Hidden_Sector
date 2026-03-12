// Models/World/Cell.cs
using System;

namespace ASTRANET.Models.World;

public class Cell
{
    public TerrainType Terrain { get; set; } = TerrainType.Empty;
    public bool Visible { get; set; } = false;
    public int EntityId { get; set; } = -1;
    public bool HasStar { get; set; } = false;
    public int StarBrightness { get; set; } = 0;

    public char GetSymbol()
    {
        // Планеты и станции видны всегда, даже вне зоны видимости
        if (Terrain == TerrainType.Planet || Terrain == TerrainType.Station)
        {
            return Terrain == TerrainType.Planet ? 'O' : 'H';
        }

        // Если есть звезда и клетка пуста или не видна, показываем звезду
        if (HasStar && (Terrain == TerrainType.Empty || !Visible))
            return GetStarChar();

        // Если клетка не видна и это не туманность – пусто
        if (!Visible && Terrain != TerrainType.Nebula)
            return ' ';

        return Terrain switch
        {
            TerrainType.Anomaly => '~',
            TerrainType.EnemyShip => 'Y',
            TerrainType.NeutralShip => 'Y',
            TerrainType.FriendlyShip => 'Y',
            TerrainType.Wreck => '♣',
            TerrainType.Asteroid => '♦',
            TerrainType.Nebula => '▒',
            _ => ' '
        };
    }

    private char GetStarChar()
    {
        return StarBrightness switch
        {
            0 => '.',
            1 => '.',
            2 => '·',
            _ => '·'
        };
    }

    public ConsoleColor GetColor()
    {
        // Планеты и станции всегда цветные
        if (Terrain == TerrainType.Planet) return ConsoleColor.Green;
        if (Terrain == TerrainType.Station) return ConsoleColor.Cyan;

        // Если есть звезда и клетка не видна (но звезда видна всегда)
        if (HasStar && (Terrain == TerrainType.Empty || !Visible))
            return GetStarColor(StarBrightness);

        if (!Visible && Terrain != TerrainType.Nebula)
            return ConsoleColor.Black;

        return Terrain switch
        {
            TerrainType.Anomaly => ConsoleColor.Magenta,
            TerrainType.EnemyShip => ConsoleColor.Red,
            TerrainType.NeutralShip => ConsoleColor.Yellow,
            TerrainType.FriendlyShip => ConsoleColor.Green,
            TerrainType.Wreck => ConsoleColor.Gray,
            TerrainType.Asteroid => ConsoleColor.DarkYellow,
            TerrainType.Nebula => ConsoleColor.DarkCyan,
            _ when HasStar => GetStarColor(StarBrightness),
            _ => ConsoleColor.Black
        };
    }

    private ConsoleColor GetStarColor(int brightness)
    {
        return brightness switch
        {
            0 => ConsoleColor.DarkGray,
            1 => ConsoleColor.Gray,
            2 => ConsoleColor.White,
            _ => ConsoleColor.White
        };
    }
}