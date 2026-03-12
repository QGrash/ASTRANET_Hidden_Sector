// Generators/InteriorGenerator.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Instances;
using ASTRANET.Models.World;
using ASTRANET.Utils;

namespace ASTRANET.Generators;

public static class InteriorGenerator
{
    private class BSPNode
    {
        public int X, Y, W, H;
        public BSPNode Left, Right;
        public bool IsLeaf => Left == null && Right == null;
        public List<Rectangle> Rooms { get; set; } = new();
    }

    public static InteriorMap Generate(InteriorType type, FactionId faction, int seed)
    {
        RandomManager.SetSeed(seed);
        InteriorMap map;

        switch (type)
        {
            case InteriorType.Shuttle:
                map = GenerateShuttle(faction);
                break;
            case InteriorType.Station:
                map = GenerateStation(faction);
                break;
            case InteriorType.PlanetaryBase:
                map = GeneratePlanetaryBase(faction);
                break;
            default:
                map = GenerateStation(faction);
                break;
        }

        PopulateInterior(map, faction, type);
        return map;
    }

    private static InteriorMap GenerateShuttle(FactionId faction)
    {
        int width = 20;
        int height = 15;
        var map = new InteriorMap(width, height);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                map.SetCell(x, y, new InteriorCell('#', false, ConsoleColor.DarkGray));

        CreateRoom(map, 2, 2, 6, 4);
        CreateRoom(map, 12, 3, 5, 5);
        CreateRoom(map, 3, 8, 5, 4);

        CreateCorridorWithDoor(map, 5, 5, 5, 8);
        CreateCorridorWithDoor(map, 7, 4, 12, 4);

        map.SetCell(15, 5, new InteriorCell('S', true, ConsoleColor.Green));
        map.Exits.Add((15, 5));

        return map;
    }

    private static InteriorMap GenerateStation(FactionId faction)
    {
        int width = 40;
        int height = 25;
        var map = new InteriorMap(width, height);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                map.SetCell(x, y, new InteriorCell('#', false, ConsoleColor.DarkGray));

        var root = new BSPNode { X = 2, Y = 2, W = width - 4, H = height - 4 };
        SplitNode(root, 6);

        var rooms = new List<Rectangle>();
        CreateRooms(root, rooms, map);
        ConnectRooms(root, rooms, map, true);

        int hangarX = 5, hangarY = 5, hangarW = 12, hangarH = 8;
        CreateRoom(map, hangarX, hangarY, hangarW, hangarH);
        rooms.Add(new Rectangle(hangarX, hangarY, hangarW, hangarH));

        foreach (var r in rooms)
        {
            if (r != rooms[rooms.Count - 1])
            {
                int x1 = hangarX + hangarW / 2;
                int y1 = hangarY + hangarH / 2;
                int x2 = r.X + r.Width / 2;
                int y2 = r.Y + r.Height / 2;
                CreateCorridorWithDoor(map, x1, y1, x2, y2);
            }
        }

        map.SetCell(hangarX + 6, hangarY + 4, new InteriorCell('S', true, ConsoleColor.Green));
        map.Exits.Add((hangarX + 6, hangarY + 4));

        return map;
    }

    private static InteriorMap GeneratePlanetaryBase(FactionId faction)
    {
        int width = 60;
        int height = 30;
        var map = new InteriorMap(width, height);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                map.SetCell(x, y, new InteriorCell('#', false, ConsoleColor.DarkGray));

        for (int x = 5; x < width - 5; x++)
            map.SetCell(x, 10, new InteriorCell('.', true, ConsoleColor.White));

        CreateRoom(map, 8, 12, 5, 4);
        CreateRoom(map, 20, 12, 6, 5);
        CreateRoom(map, 35, 12, 7, 5);
        CreateRoom(map, 45, 12, 4, 4);

        for (int y = 15; y < 25; y++)
        {
            map.SetCell(15, y, new InteriorCell('.', true, ConsoleColor.White));
            map.SetCell(30, y, new InteriorCell('.', true, ConsoleColor.White));
        }

        CreateRoom(map, 12, 20, 5, 4);
        CreateRoom(map, 27, 20, 5, 4);

        AddDoor(map, 10, 15);
        AddDoor(map, 22, 15);
        AddDoor(map, 38, 15);
        AddDoor(map, 47, 15);
        AddDoor(map, 14, 22);
        AddDoor(map, 29, 22);

        map.SetCell(5, 10, new InteriorCell('S', true, ConsoleColor.Green));
        map.Exits.Add((5, 10));

        return map;
    }

    private static void CreateRoom(InteriorMap map, int x, int y, int w, int h)
    {
        for (int ry = y; ry < y + h; ry++)
            for (int rx = x; rx < x + w; rx++)
                map.SetCell(rx, ry, new InteriorCell('.', true, ConsoleColor.White));
    }

    private static void CreateCorridor(InteriorMap map, int x1, int y1, int x2, int y2)
    {
        if (RandomManager.NextDouble() > 0.5)
        {
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                if (map.GetCell(x, y1).Tile == '#')
                    map.SetCell(x, y1, new InteriorCell('.', true, ConsoleColor.White));
            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                if (map.GetCell(x2, y).Tile == '#')
                    map.SetCell(x2, y, new InteriorCell('.', true, ConsoleColor.White));
        }
        else
        {
            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                if (map.GetCell(x1, y).Tile == '#')
                    map.SetCell(x1, y, new InteriorCell('.', true, ConsoleColor.White));
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                if (map.GetCell(x, y2).Tile == '#')
                    map.SetCell(x, y2, new InteriorCell('.', true, ConsoleColor.White));
        }
    }

    private static void CreateCorridorWithDoor(InteriorMap map, int x1, int y1, int x2, int y2)
    {
        CreateCorridor(map, x1, y1, x2, y2);
        int doorX = (x1 + x2) / 2;
        int doorY = (y1 + y2) / 2;
        if (map.GetCell(doorX, doorY).Tile == '.')
            map.SetCell(doorX, doorY, new InteriorCell('+', true, ConsoleColor.Yellow));
    }

    private static void AddDoor(InteriorMap map, int x, int y)
    {
        if (map.GetCell(x, y).Tile == '.')
            map.SetCell(x, y, new InteriorCell('+', true, ConsoleColor.Yellow));
    }

    private static void SplitNode(BSPNode node, int minSize)
    {
        if (node.W < minSize * 2 || node.H < minSize * 2) return;

        bool splitHoriz = RandomManager.NextDouble() > 0.5;
        if (splitHoriz)
        {
            int split = RandomManager.Next(minSize, node.H - minSize);
            node.Left = new BSPNode { X = node.X, Y = node.Y, W = node.W, H = split };
            node.Right = new BSPNode { X = node.X, Y = node.Y + split, W = node.W, H = node.H - split };
        }
        else
        {
            int split = RandomManager.Next(minSize, node.W - minSize);
            node.Left = new BSPNode { X = node.X, Y = node.Y, W = split, H = node.H };
            node.Right = new BSPNode { X = node.X + split, Y = node.Y, W = node.W - split, H = node.H };
        }

        SplitNode(node.Left, minSize);
        SplitNode(node.Right, minSize);
    }

    private static void CreateRooms(BSPNode node, List<Rectangle> rooms, InteriorMap map)
    {
        if (node == null) return;
        if (node.IsLeaf)
        {
            int roomW = RandomManager.Next(4, node.W - 1);
            int roomH = RandomManager.Next(4, node.H - 1);
            int roomX = node.X + RandomManager.Next(1, node.W - roomW);
            int roomY = node.Y + RandomManager.Next(1, node.H - roomH);

            var rect = new Rectangle(roomX, roomY, roomW, roomH);
            rooms.Add(rect);
            node.Rooms.Add(rect);

            for (int y = roomY; y < roomY + roomH; y++)
                for (int x = roomX; x < roomX + roomW; x++)
                    map.SetCell(x, y, new InteriorCell('.', true, ConsoleColor.White));
        }
        else
        {
            CreateRooms(node.Left, rooms, map);
            CreateRooms(node.Right, rooms, map);
        }
    }

    private static void ConnectRooms(BSPNode node, List<Rectangle> rooms, InteriorMap map, bool addDoors)
    {
        if (node == null || node.IsLeaf) return;

        if (node.Left != null && node.Right != null)
        {
            var leftRooms = new List<Rectangle>();
            var rightRooms = new List<Rectangle>();
            CollectRooms(node.Left, leftRooms);
            CollectRooms(node.Right, rightRooms);

            if (leftRooms.Count > 0 && rightRooms.Count > 0)
            {
                var r1 = leftRooms[RandomManager.Next(leftRooms.Count)];
                var r2 = rightRooms[RandomManager.Next(rightRooms.Count)];
                int x1 = r1.X + r1.Width / 2;
                int y1 = r1.Y + r1.Height / 2;
                int x2 = r2.X + r2.Width / 2;
                int y2 = r2.Y + r2.Height / 2;

                if (addDoors)
                    CreateCorridorWithDoor(map, x1, y1, x2, y2);
                else
                    CreateCorridor(map, x1, y1, x2, y2);
            }
        }

        ConnectRooms(node.Left, rooms, map, addDoors);
        ConnectRooms(node.Right, rooms, map, addDoors);
    }

    private static void CollectRooms(BSPNode node, List<Rectangle> rooms)
    {
        if (node == null) return;
        if (node.IsLeaf)
            rooms.AddRange(node.Rooms);
        else
        {
            CollectRooms(node.Left, rooms);
            CollectRooms(node.Right, rooms);
        }
    }

    private static void PopulateInterior(InteriorMap map, FactionId faction, InteriorType type)
    {
        int npcCount = type == InteriorType.Shuttle ? 1 : (type == InteriorType.Station ? 3 : 5);

        for (int i = 0; i < npcCount; i++)
        {
            int tries = 100;
            while (tries-- > 0)
            {
                int x = RandomManager.Next(map.Width);
                int y = RandomManager.Next(map.Height);
                var cell = map.GetCell(x, y);
                if (cell != null && cell.Tile == '.' && map.GetNpcAt(x, y) == null)
                {
                    var npc = new InteriorNpc
                    {
                        Id = i + 1000,
                        Name = GenerateNpcName(faction),
                        Type = DetermineNpcType(faction, i),
                        Faction = faction,
                        Behavior = RandomManager.NextDouble() < 0.3 ? NpcBehavior.Patrol : NpcBehavior.Stationary,
                        X = x,
                        Y = y,
                        Health = 30,
                        MaxHealth = 30,
                        DialogueId = DetermineDialogueId(faction, DetermineNpcType(faction, i))
                    };

                    if (npc.Behavior == NpcBehavior.Patrol)
                    {
                        npc.PatrolTargetX = x + RandomManager.Next(-3, 4);
                        npc.PatrolTargetY = y + RandomManager.Next(-3, 4);
                    }

                    if (npc.Type == ASTRANET.NpcType.Enemy)
                    {
                        npc.Inventory.Add(new ItemInstance("medkit_basic", 1));
                    }
                    else if (npc.Type == ASTRANET.NpcType.Friendly)
                    {
                        npc.Inventory.Add(new ItemInstance("medkit_basic", 1));
                    }

                    map.Npcs.Add(npc);
                    break;
                }
            }
        }

        int containerCount = type == InteriorType.Shuttle ? 1 : (type == InteriorType.Station ? 3 : 6);
        for (int i = 0; i < containerCount; i++)
        {
            int tries = 100;
            while (tries-- > 0)
            {
                int x = RandomManager.Next(map.Width);
                int y = RandomManager.Next(map.Height);
                var cell = map.GetCell(x, y);
                if (cell != null && cell.Tile == '.' && !map.Containers.Contains((x, y)))
                {
                    map.SetCell(x, y, new InteriorCell('C', true, ConsoleColor.Yellow));
                    map.Containers.Add((x, y));
                    break;
                }
            }
        }
    }

    private static string GenerateNpcName(FactionId faction)
    {
        return faction.ToString() + " Guard";
    }

    private static ASTRANET.NpcType DetermineNpcType(FactionId faction, int index)
    {
        if (faction == FactionId.CrimsonVoyagers || faction == FactionId.VoidDragons || faction == FactionId.CultOfVoid)
            return ASTRANET.NpcType.Enemy;
        if (faction == FactionId.SolarFederation || faction == FactionId.GreenCrossFoundation)
            return ASTRANET.NpcType.Friendly;
        return index == 0 ? ASTRANET.NpcType.Neutral : ASTRANET.NpcType.Enemy;
    }

    private static string DetermineDialogueId(FactionId faction, ASTRANET.NpcType type)
    {
        // Пример: дружественные NPC могут иметь диалог "federation_officer", нейтралы – "brog_scout", враги – null
        if (type == ASTRANET.NpcType.Enemy) return null;
        if (type == ASTRANET.NpcType.Friendly) return "federation_officer";
        if (type == ASTRANET.NpcType.Neutral) return "brog_scout";
        return null;
    }
}