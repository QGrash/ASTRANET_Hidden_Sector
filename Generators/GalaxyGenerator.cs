// Generators/GalaxyGenerator.cs
using System.Collections.Generic;
using ASTRANET.Models.World;
using ASTRANET.Utils;

namespace ASTRANET.Generators;

public static class GalaxyGenerator
{
    public static Galaxy Generate(int seed)
    {
        RandomManager.SetSeed(seed);
        var galaxy = new Galaxy { Seed = seed };
        var systems = new List<StarSystem>();

        // Сектор Альфа
        systems.Add(new StarSystem(0, "Альфа Центавра", SystemType.Habitable, 20, 8));
        systems.Add(new StarSystem(1, "Альфа-2", SystemType.Desert, 35, 12));
        systems.Add(new StarSystem(2, "Альфа-3", SystemType.Hostile, 15, 18));

        // Сектор Бета
        systems.Add(new StarSystem(3, "Бета Гидры", SystemType.Habitable, 85, 10));
        systems.Add(new StarSystem(4, "Бета-7", SystemType.Anomaly, 100, 15));
        systems.Add(new StarSystem(5, "Бета-9", SystemType.Desert, 75, 20));

        // Сектор Гамма
        systems.Add(new StarSystem(6, "Гамма Дракона", SystemType.Hostile, 25, 30));
        systems.Add(new StarSystem(7, "Гамма-4", SystemType.Anomaly, 40, 32));
        systems.Add(new StarSystem(8, "Гамма-12", SystemType.Habitable, 10, 35));

        // Сектор Дельта
        systems.Add(new StarSystem(9, "Дельта Ориона", SystemType.Habitable, 90, 28));
        systems.Add(new StarSystem(10, "Дельта-6", SystemType.Desert, 105, 33));
        systems.Add(new StarSystem(11, "Дельта-11", SystemType.Hostile, 80, 35));

        // Сектор Зета – скрыт
        systems.Add(new StarSystem(12, "Зета Сектор", SystemType.Hidden, 55, 20) { Hidden = true });

        // Двунаправленные связи
        AddBidirectional(systems[0], systems[1]);
        AddBidirectional(systems[0], systems[2]);
        AddBidirectional(systems[1], systems[3]);
        AddBidirectional(systems[2], systems[6]);

        AddBidirectional(systems[3], systems[4]);
        AddBidirectional(systems[3], systems[5]);
        AddBidirectional(systems[4], systems[9]);
        AddBidirectional(systems[5], systems[7]);

        AddBidirectional(systems[6], systems[7]);
        AddBidirectional(systems[6], systems[8]);
        AddBidirectional(systems[7], systems[10]);
        AddBidirectional(systems[8], systems[12]);

        AddBidirectional(systems[9], systems[10]);
        AddBidirectional(systems[9], systems[11]);
        AddBidirectional(systems[10], systems[11]);
        AddBidirectional(systems[11], systems[12]);

        AddBidirectional(systems[12], systems[2]);
        AddBidirectional(systems[12], systems[8]);
        AddBidirectional(systems[12], systems[11]);

        galaxy.Systems = systems;
        return galaxy;
    }

    private static void AddBidirectional(StarSystem a, StarSystem b)
    {
        if (!a.ConnectedSystems.Contains(b.Id))
            a.ConnectedSystems.Add(b.Id);
        if (!b.ConnectedSystems.Contains(a.Id))
            b.ConnectedSystems.Add(a.Id);
    }
}