using System;
using System.Collections.Generic;
using System.Linq;

namespace ASTRANET_Hidden_Sector.World
{
    public class WorldGenerator
    {
        private Random random;
        private const int MIN_DISTANCE = 15;
        private const int BORDER_PADDING = 10;

        public WorldGenerator(int seed)
        {
            random = new Random(seed);
        }

        public List<Sector> GenerateSectors()
        {
            var sectors = new List<Sector>
            {
                new Sector("Альфа", false, 20, 20),
                new Sector("Бета", false, 80, 20),
                new Sector("Гамма", false, 20, 80),
                new Sector("Омега", false, 80, 80),
                new Sector("Зета", true, 50, 50)
            };

            sectors[0].ConnectedSectors.Add(sectors[1]); // Альфа-Бета
            sectors[0].ConnectedSectors.Add(sectors[2]); // Альфа-Гамма
            sectors[0].ConnectedSectors.Add(sectors[4]); // Альфа-Зета

            sectors[1].ConnectedSectors.Add(sectors[0]); // Бета-Альфа
            sectors[1].ConnectedSectors.Add(sectors[3]); // Бета-Омега
            sectors[1].ConnectedSectors.Add(sectors[4]); // Бета-Зета

            sectors[2].ConnectedSectors.Add(sectors[0]); // Гамма-Альфа
            sectors[2].ConnectedSectors.Add(sectors[3]); // Гамма-Омега
            sectors[2].ConnectedSectors.Add(sectors[4]); // Гамма-Зета

            sectors[3].ConnectedSectors.Add(sectors[1]); // Омега-Бета
            sectors[3].ConnectedSectors.Add(sectors[2]); // Омега-Гамма
            sectors[3].ConnectedSectors.Add(sectors[4]); // Омега-Зета

            foreach (var sector in sectors)
            {
                GenerateSystems(sector);
                GenerateConnections(sector);
            }

            return sectors;
        }

        private void GenerateSystems(Sector sector)
        {
            int count = random.Next(10, 21);
            int maxAttempts = 1000;

            for (int i = 0; i < count; i++)
            {
                int attempts = 0;
                bool placed = false;

                while (!placed && attempts < maxAttempts)
                {
                    int x = random.Next(BORDER_PADDING, sector.Width - BORDER_PADDING);
                    int y = random.Next(BORDER_PADDING, sector.Height - BORDER_PADDING);

                    bool tooClose = false;
                    foreach (var sys in sector.Systems)
                    {
                        double dist = Math.Sqrt((sys.X - x) * (sys.X - x) + (sys.Y - y) * (sys.Y - y));
                        if (dist < MIN_DISTANCE)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        var system = new StarSystem
                        {
                            Name = $"{sector.Name}-{i + 1}",
                            X = x,
                            Y = y,
                            Type = (SystemType)random.Next(0, 5)
                        };
                        sector.Systems.Add(system);
                        placed = true;
                    }

                    attempts++;
                }

                if (!placed)
                {
                    var fallbackSystem = new StarSystem
                    {
                        Name = $"{sector.Name}-{i + 1}",
                        X = random.Next(0, sector.Width),
                        Y = random.Next(0, sector.Height),
                        Type = (SystemType)random.Next(0, 5)
                    };
                    sector.Systems.Add(fallbackSystem);
                }
            }
            foreach (var system in sector.Systems)
            {
                system.LocalMap = LocalMapGenerator.Generate(system);
            }
        }

        private void GenerateConnections(Sector sector)
        {
            var systems = sector.Systems;
            if (systems.Count < 2) return;

            var mst = new List<StarSystem>();
            var first = systems[0];
            mst.Add(first);

            while (mst.Count < systems.Count)
            {
                StarSystem bestFrom = null;
                StarSystem bestTo = null;
                double bestDist = double.MaxValue;

                foreach (var from in mst)
                {
                    foreach (var to in systems)
                    {
                        if (mst.Contains(to)) continue;
                        double dist = Distance(from, to);
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                            bestFrom = from;
                            bestTo = to;
                        }
                    }
                }

                if (bestFrom != null && bestTo != null)
                {
                    bestFrom.ConnectedSystems.Add(bestTo);
                    bestTo.ConnectedSystems.Add(bestFrom);
                    mst.Add(bestTo);
                }
                else break;
            }

            int extraEdges = (int)(systems.Count * 0.2);
            for (int i = 0; i < extraEdges; i++)
            {
                var a = systems[random.Next(systems.Count)];
                var b = systems[random.Next(systems.Count)];
                if (a != b && !a.ConnectedSystems.Contains(b))
                {
                    a.ConnectedSystems.Add(b);
                    b.ConnectedSystems.Add(b);
                }
            }
        }

        private double Distance(StarSystem a, StarSystem b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
    }
}