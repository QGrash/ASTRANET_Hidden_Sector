using System;
using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.World
{
    public static class LocalMapGenerator
    {
        private static Random random = new Random();

        public static LocalMap Generate(StarSystem system)
        {
            var map = new LocalMap();

            int planets = 0, stations = 0, enemies = 0, anomalies = 0, resources = 0;

            switch (system.Type)
            {
                case SystemType.Habitable:
                    planets = random.Next(2, 5);
                    stations = random.Next(1, 4);
                    enemies = random.Next(0, 3);
                    anomalies = random.Next(0, 2);
                    resources = random.Next(1, 4);
                    break;
                case SystemType.Desert:
                    planets = random.Next(1, 4);
                    stations = random.Next(0, 2);
                    enemies = random.Next(1, 4);
                    anomalies = random.Next(1, 3);
                    resources = random.Next(2, 5);
                    break;
                case SystemType.Hostile:
                    planets = random.Next(0, 3);
                    stations = random.Next(0, 1);
                    enemies = random.Next(3, 7);
                    anomalies = random.Next(1, 3);
                    resources = random.Next(0, 3);
                    break;
                case SystemType.Anomalous:
                    planets = random.Next(1, 3);
                    stations = random.Next(0, 2);
                    enemies = random.Next(1, 4);
                    anomalies = random.Next(3, 6);
                    resources = random.Next(1, 3);
                    break;
                case SystemType.Hidden:
                    planets = random.Next(1, 4);
                    stations = random.Next(0, 2);
                    enemies = random.Next(1, 5);
                    anomalies = random.Next(1, 4);
                    resources = random.Next(1, 4);
                    break;
            }

            PlaceLocations(map, LocationType.Planet, planets, "Планета", ConsoleColor.Green);
            PlaceLocations(map, LocationType.Station, stations, "Станция", ConsoleColor.Cyan, "trader_01");
            PlaceLocations(map, LocationType.Enemy, enemies, "Враг", ConsoleColor.Red);
            PlaceLocations(map, LocationType.Anomaly, anomalies, "Аномалия", ConsoleColor.Magenta);
            PlaceLocations(map, LocationType.Resources, resources, "Ресурсы", ConsoleColor.Yellow);

            map.PlayerX = LocalMap.Width / 2;
            map.PlayerY = LocalMap.Height / 2;
            map.UpdateVisibility();

            return map;
        }

        private static void PlaceLocations(LocalMap map, LocationType type, int count, string baseName, ConsoleColor color, string dialogueId = "")
        {
            for (int i = 0; i < count; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(0, LocalMap.Width);
                    y = random.Next(0, LocalMap.Height);
                } while (map.GetCell(x, y)?.Entity != null);

                var loc = new Location
                {
                    Name = $"{baseName} {i + 1}",
                    Description = $"Описание {baseName.ToLower()}",
                    Type = type,
                    X = x,
                    Y = y
                };
                // Для станций передаём dialogueId, для остальных — пустую строку
                var entity = new StaticEntity(loc, type == LocationType.Station ? dialogueId : "");
                map.SetCell(x, y, entity);
            }
        }
    }
}