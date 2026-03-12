// Generators/LocalMapGenerator.cs
using System.Collections.Generic;
using ASTRANET.Models.Entities;
using ASTRANET.Models.World;
using ASTRANET.Utils;

namespace ASTRANET.Generators;

public static class LocalMapGenerator
{
    public static LocalMap Generate(StarSystem system)
    {
        var map = new LocalMap();

        // Планеты
        for (int i = 0; i < 5; i++)
        {
            int x = RandomManager.Next(LocalMap.Width);
            int y = RandomManager.Next(LocalMap.Height);
            var cell = map.GetCell(x, y);
            if (cell.Terrain == TerrainType.Empty)
            {
                cell.Terrain = TerrainType.Planet;
            }
        }

        // Станция
        int sx, sy;
        do
        {
            sx = RandomManager.Next(LocalMap.Width);
            sy = RandomManager.Next(LocalMap.Height);
        } while (map.GetCell(sx, sy).Terrain != TerrainType.Empty);
        map.GetCell(sx, sy).Terrain = TerrainType.Station;

        // Генерация NPC
        GenerateNpcs(map, system);

        // Генерация звёзд
        for (int y = 0; y < LocalMap.Height; y++)
        {
            for (int x = 0; x < LocalMap.Width; x++)
            {
                if (map.GetCell(x, y).Terrain == TerrainType.Empty && RandomManager.NextDouble() < 0.3)
                {
                    map.GetCell(x, y).HasStar = true;
                    map.GetCell(x, y).StarBrightness = RandomManager.Next(4);
                }
            }
        }

        return map;
    }

    private static void GenerateNpcs(LocalMap map, StarSystem system)
    {
        int count = RandomManager.Next(3, 7);

        for (int i = 0; i < count; i++)
        {
            int x, y;
            int tries = 100;
            do
            {
                x = RandomManager.Next(LocalMap.Width);
                y = RandomManager.Next(LocalMap.Height);
                tries--;
            } while (tries > 0 && map.GetCell(x, y).Terrain != TerrainType.Empty);

            if (tries == 0) continue;

            ASTRANET.NpcType type;
            if (system.Type == SystemType.Hostile)
                type = ASTRANET.NpcType.Enemy;
            else if (system.Type == SystemType.Habitable)
                type = RandomManager.NextDouble() < 0.3 ? ASTRANET.NpcType.Enemy : (RandomManager.NextDouble() < 0.5 ? ASTRANET.NpcType.Neutral : ASTRANET.NpcType.Friendly);
            else
                type = RandomManager.NextDouble() < 0.5 ? ASTRANET.NpcType.Enemy : ASTRANET.NpcType.Neutral;

            FactionId faction = type == ASTRANET.NpcType.Enemy ? FactionId.CrimsonVoyagers : (type == ASTRANET.NpcType.Friendly ? FactionId.SolarFederation : FactionId.BrogNomads);

            var npc = new SpaceNpc
            {
                Id = i + 1000,
                Name = GetNpcName(type),
                Type = type,
                Faction = faction,
                X = x,
                Y = y,
                Health = 50,
                MaxHealth = 50,
                Attack = 8,
                Defense = 3,
                IsPatrol = RandomManager.NextDouble() < 0.5,
                DialogueId = type == ASTRANET.NpcType.Friendly ? "federation_officer" : (type == ASTRANET.NpcType.Neutral ? "brog_scout" : null)
            };

            if (npc.IsPatrol)
            {
                npc.PatrolTargetX = x + RandomManager.Next(-3, 4);
                npc.PatrolTargetY = y + RandomManager.Next(-3, 4);
            }

            map.Npcs.Add(npc);
        }
    }

    private static string GetNpcName(ASTRANET.NpcType type)
    {
        return type switch
        {
            ASTRANET.NpcType.Enemy => "Пират",
            ASTRANET.NpcType.Neutral => "Торговец",
            ASTRANET.NpcType.Friendly => "Патрульный",
            _ => "Неизвестный"
        };
    }
}