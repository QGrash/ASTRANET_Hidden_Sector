// Screens/LocalMapScreen.cs
using ASTRANET.Core;
using ASTRANET.Generators;
using ASTRANET.Managers;
using ASTRANET.Models.Combat;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Prototypes;
using ASTRANET.Models.World;
using ASTRANET.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASTRANET.Screens;

public class LocalMapScreen : BaseScreen
{
    private readonly WorldManager _worldManager;
    private readonly Player _player;
    private readonly List<(string text, ConsoleColor color)> _hints;
    private List<(int x, int y)> _scanCircle;
    private const int MapOffsetX = 2;
    private const int MapOffsetY = 3;

    public LocalMapScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _worldManager = DI.Resolve<WorldManager>();
        _player = DI.Resolve<Player>();

        _player.X = LocalMap.Width / 2;
        _player.Y = LocalMap.Height / 2;

        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓/←/→ - движение (1 топлива)", ConsoleColor.Gray),
            ("T - взаимодействие", ConsoleColor.Gray),
            ("I - инвентарь", ConsoleColor.Gray),
            ("F - SOS (20 энергии)", ConsoleColor.Gray),
            ("Space - пропустить ход", ConsoleColor.Gray),
            ("Backspace - выйти на карту галактики", ConsoleColor.Gray)
        };

        _scanCircle = Geometry.BresenhamCircle(_player.X, _player.Y, LocalMap.VisibilityRadius, dashed: true);

        // Подписываемся на событие SOS
        EventBus.Subscribe<SOSBeaconActivatedEvent>(OnSOSActivated);
    }

    private void OnSOSActivated(SOSBeaconActivatedEvent evt)
    {
        var map = _worldManager.CurrentLocalMap;
        if (map == null) return;

        List<(int x, int y)> candidates = new();
        for (int dy = -2; dy <= 2; dy++)
        {
            for (int dx = -2; dx <= 2; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = _player.X + dx;
                int ny = _player.Y + dy;
                if (nx >= 0 && nx < LocalMap.Width && ny >= 0 && ny < LocalMap.Height)
                {
                    var cell = map.GetCell(nx, ny);
                    if (cell != null && (cell.Terrain == TerrainType.Empty || cell.Terrain == TerrainType.Nebula) && map.GetNpcAt(nx, ny) == null)
                    {
                        candidates.Add((nx, ny));
                    }
                }
            }
        }

        if (candidates.Count == 0)
        {
            UIManager.ShowMessage("Нет места для спасательного корабля!");
            return;
        }

        var (spawnX, spawnY) = candidates[RandomManager.Next(candidates.Count)];

        double r = RandomManager.NextDouble();
        ASTRANET.NpcType npcType;
        string dialogueId;
        FactionId faction;

        if (r < 0.7)
        {
            npcType = ASTRANET.NpcType.Friendly;
            dialogueId = "rescue_friendly";
            faction = FactionId.SolarFederation;
        }
        else if (r < 0.9)
        {
            npcType = ASTRANET.NpcType.Neutral;
            dialogueId = "rescue_neutral";
            faction = FactionId.MercArmory;
        }
        else
        {
            npcType = ASTRANET.NpcType.Enemy;
            dialogueId = null;
            faction = FactionId.CrimsonVoyagers;
        }

        var npc = new SpaceNpc
        {
            Id = map.Npcs.Count + 2000,
            Name = npcType == ASTRANET.NpcType.Enemy ? "Пират-мародёр" : (npcType == ASTRANET.NpcType.Friendly ? "Спасатель" : "Торговец"),
            Type = npcType,
            Faction = faction,
            X = spawnX,
            Y = spawnY,
            Health = 50,
            MaxHealth = 50,
            Attack = 8,
            Defense = 3,
            DialogueId = dialogueId,
            IsPatrol = npcType != ASTRANET.NpcType.Enemy
        };

        map.AddNpc(npc);
        UIManager.ShowMessage("Сигнал SOS принят! К вам приближается корабль.");
    }

    public override void Update(float deltaTime)
    {
        var map = _worldManager.CurrentLocalMap;
        if (map == null) return;

        for (int y = 0; y < LocalMap.Height; y++)
        {
            for (int x = 0; x < LocalMap.Width; x++)
            {
                var cell = map.GetCell(x, y);
                if (cell.HasStar && RandomManager.NextDouble() < 0.001)
                {
                    cell.StarBrightness = (cell.StarBrightness + 1) % 4;
                }
            }
        }
    }

    public override void Render()
    {
        DrawShipStatus();

        var map = _worldManager.CurrentLocalMap;
        if (map == null) return;

        for (int y = 0; y < LocalMap.Height; y++)
            for (int x = 0; x < LocalMap.Width; x++)
                UIManager.SetPixel(x + MapOffsetX, y + MapOffsetY, ' ', ConsoleColor.Black);

        int left = MapOffsetX - 1;
        int right = MapOffsetX + LocalMap.Width;
        int top = MapOffsetY - 1;
        int bottom = MapOffsetY + LocalMap.Height;

        for (int x = left; x <= right; x++)
        {
            SetPixelIfInBounds(x, top, '-', ConsoleColor.DarkGray);
            SetPixelIfInBounds(x, bottom, '-', ConsoleColor.DarkGray);
        }
        for (int y = top; y <= bottom; y++)
        {
            SetPixelIfInBounds(left, y, '|', ConsoleColor.DarkGray);
            SetPixelIfInBounds(right, y, '|', ConsoleColor.DarkGray);
        }
        SetPixelIfInBounds(left, top, '+', ConsoleColor.DarkGray);
        SetPixelIfInBounds(right, top, '+', ConsoleColor.DarkGray);
        SetPixelIfInBounds(left, bottom, '+', ConsoleColor.DarkGray);
        SetPixelIfInBounds(right, bottom, '+', ConsoleColor.DarkGray);

        map.SetVisible(_player.X, _player.Y);

        for (int y = 0; y < LocalMap.Height; y++)
        {
            for (int x = 0; x < LocalMap.Width; x++)
            {
                var cell = map.GetCell(x, y);
                UIManager.SetPixel(x + MapOffsetX, y + MapOffsetY, cell.GetSymbol(), cell.GetColor());
            }
        }

        foreach (var npc in map.Npcs.Where(n => n.IsAlive))
        {
            if (IsVisible(npc.X, npc.Y, map))
            {
                char symbol = 'Y';
                ConsoleColor color = npc.Type == ASTRANET.NpcType.Enemy ? ConsoleColor.Red :
                                     npc.Type == ASTRANET.NpcType.Neutral ? ConsoleColor.Yellow :
                                     ConsoleColor.Green;
                UIManager.SetPixel(npc.X + MapOffsetX, npc.Y + MapOffsetY, symbol, color);
            }
        }

        UIManager.SetPixel(_player.X + MapOffsetX, _player.Y + MapOffsetY, 'Y', ConsoleColor.White);

        _scanCircle = Geometry.BresenhamCircle(_player.X, _player.Y, LocalMap.VisibilityRadius, dashed: true);
        foreach (var (cx, cy) in _scanCircle)
        {
            if (cx >= 0 && cx < LocalMap.Width && cy >= 0 && cy < LocalMap.Height)
            {
                var cell = map.GetCell(cx, cy);
                if (cell.Terrain == TerrainType.Empty && !cell.HasStar)
                    UIManager.SetPixel(cx + MapOffsetX, cy + MapOffsetY, '·', ConsoleColor.DarkCyan);
            }
        }

        UIManager.DrawHints(_hints);
    }

    private bool IsVisible(int x, int y, LocalMap map)
    {
        var cell = map.GetCell(x, y);
        return cell != null && cell.Visible;
    }

    private void DrawShipStatus()
    {
        int x = 2;
        int y = 0;
        UIManager.DrawString(x, y, $"Корпус: {_player.Hull}/{_player.MaxHull}  ", ConsoleColor.Red);
        x += 15;
        UIManager.DrawString(x, y, $"Щиты: {_player.Shields}/{_player.MaxShields}  ", ConsoleColor.Cyan);
        x += 15;
        UIManager.DrawString(x, y, $"Энергия: {_player.Energy}/{_player.MaxEnergy}  ", ConsoleColor.Yellow);
        x += 15;
        UIManager.DrawString(x, y, $"Топливо: {_player.Fuel}/{_player.MaxFuel}  ", ConsoleColor.Green);
    }

    private void SetPixelIfInBounds(int x, int y, char c, ConsoleColor color)
    {
        if (x >= 0 && x < UIManager.Width && y >= 0 && y < UIManager.Height)
            UIManager.SetPixel(x, y, c, color);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        var map = _worldManager.CurrentLocalMap;
        if (map == null) return false;

        bool actionPerformed = false;

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.DownArrow:
            case ConsoleKey.LeftArrow:
            case ConsoleKey.RightArrow:
                actionPerformed = TryMove(keyInfo.Key, map);
                break;
            case ConsoleKey.T:
                actionPerformed = Interact(map);
                break;
            case ConsoleKey.I:
                return true;
            case ConsoleKey.F:
                _player.ActivateSOS();
                return true;
            case ConsoleKey.Spacebar:
                SkipTurn();
                actionPerformed = true;
                break;
            case ConsoleKey.Backspace:
                ScreenManager.PopScreen();
                return true;
            default:
                return false;
        }

        if (actionPerformed)
        {
            ProcessNPCTurn(map);
        }
        return true;
    }

    private bool TryMove(ConsoleKey key, LocalMap map)
    {
        int newX = _player.X, newY = _player.Y;
        switch (key)
        {
            case ConsoleKey.UpArrow: newY--; break;
            case ConsoleKey.DownArrow: newY++; break;
            case ConsoleKey.LeftArrow: newX--; break;
            case ConsoleKey.RightArrow: newX++; break;
        }

        if (newX < 0 || newX >= LocalMap.Width || newY < 0 || newY >= LocalMap.Height)
            return false;

        if (_player.Fuel < 1)
        {
            UIManager.ShowMessage("Недостаточно топлива для движения!");
            return false;
        }

        var targetCell = map.GetCell(newX, newY);
        if (targetCell == null) return false;

        if (map.GetNpcAt(newX, newY) != null)
        {
            UIManager.ShowMessage("Путь преграждён.");
            return false;
        }

        if (targetCell.Terrain == TerrainType.Empty || targetCell.Terrain == TerrainType.Nebula)
        {
            _player.ConsumeFuel(1);
            _player.X = newX;
            _player.Y = newY;
            return true;
        }
        return false;
    }

    private bool Interact(LocalMap map)
    {
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };
        List<SpaceNpc> adjacentNpcs = new();

        for (int i = 0; i < 4; i++)
        {
            int nx = _player.X + dx[i];
            int ny = _player.Y + dy[i];
            var npc = map.GetNpcAt(nx, ny);
            if (npc != null)
                adjacentNpcs.Add(npc);
        }

        if (adjacentNpcs.Count > 0)
        {
            var target = adjacentNpcs[0];
            if (target.Type == ASTRANET.NpcType.Enemy)
            {
                StartCombat(target);
            }
            else
            {
                StartDialogue(target);
            }
            return true;
        }

        for (int i = 0; i < 4; i++)
        {
            int nx = _player.X + dx[i];
            int ny = _player.Y + dy[i];
            if (nx < 0 || nx >= LocalMap.Width || ny < 0 || ny >= LocalMap.Height)
                continue;
            var cell = map.GetCell(nx, ny);
            if (cell.Terrain == TerrainType.Station)
            {
                var interior = InteriorGenerator.Generate(InteriorType.Station, FactionId.SolarFederation, RandomManager.Next());
                var interiorScreen = new InteriorScreen(UIManager, ScreenManager, InputManager, interior, _player);
                ScreenManager.PushScreen(interiorScreen);
                return true;
            }
            else if (cell.Terrain == TerrainType.Wreck)
            {
                UIManager.ShowMessage("Обломки. Пока ничего нет.");
                return true;
            }
        }
        return false;
    }

    private void StartCombat(SpaceNpc target)
    {
        var enemy = new SpaceShip(target.Id.ToString(), target.Name, target.Faction, 80, 40, 100);
        var itemManager = DI.Resolve<ItemManager>();
        var weaponProto = itemManager.GetPrototype("weapon_laser");
        if (weaponProto != null)
        {
            enemy.Weapons.Add(new SpaceWeapon(weaponProto));
        }
        var enemies = new List<SpaceShip> { enemy };
        var combatManager = new SpaceCombatManager(_player, enemies, _worldManager);
        combatManager.StartCombat();
        var combatScreen = new SpaceCombatScreen(UIManager, ScreenManager, InputManager, combatManager);
        ScreenManager.PushScreen(combatScreen);
    }

    private void StartDialogue(SpaceNpc target)
    {
        if (string.IsNullOrEmpty(target.DialogueId))
        {
            UIManager.ShowMessage($"{target.Name} не хочет разговаривать.");
            return;
        }
        var dialogueManager = DI.Resolve<DialogueManager>();
        if (dialogueManager.StartDialogue(target.DialogueId))
        {
            var dialogueScreen = new DialogueScreen(UIManager, ScreenManager, InputManager, target.Name);
            ScreenManager.PushScreen(dialogueScreen);
        }
        else
        {
            UIManager.ShowMessage($"Ошибка: диалог {target.DialogueId} не найден!");
        }
    }

    private void SkipTurn()
    {
        _player.Regenerate();
        UIManager.ShowMessage("Ход пропущен. Энергия и щиты восстановлены.");
    }

    private void ProcessNPCTurn(LocalMap map)
    {
        foreach (var npc in map.Npcs.Where(n => n.IsAlive && n.Type == ASTRANET.NpcType.Enemy))
        {
            if (Math.Abs(npc.X - _player.X) <= 1 && Math.Abs(npc.Y - _player.Y) <= 1)
            {
                int damage = npc.Attack - _player.GetTotalDefense();
                if (damage < 1) damage = 1;
                _player.TakeDamage(damage);
                UIManager.ShowMessage($"{npc.Name} атакует вас, нанося {damage} урона.");
                continue;
            }

            int dx = 0, dy = 0;
            if (npc.X < _player.X) dx = 1;
            else if (npc.X > _player.X) dx = -1;
            else if (npc.Y < _player.Y) dy = 1;
            else if (npc.Y > _player.Y) dy = -1;

            int newX = npc.X + dx;
            int newY = npc.Y + dy;

            if (CanNPCStep(newX, newY, map))
            {
                npc.X = newX;
                npc.Y = newY;
            }
        }

        foreach (var npc in map.Npcs.Where(n => n.IsAlive && n.Type != ASTRANET.NpcType.Enemy && n.IsPatrol))
        {
            int dx = 0, dy = 0;
            if (npc.X < npc.PatrolTargetX) dx = 1;
            else if (npc.X > npc.PatrolTargetX) dx = -1;
            else if (npc.Y < npc.PatrolTargetY) dy = 1;
            else if (npc.Y > npc.PatrolTargetY) dy = -1;

            int newX = npc.X + dx;
            int newY = npc.Y + dy;

            if (newX == npc.PatrolTargetX && newY == npc.PatrolTargetY)
            {
                npc.PatrolTargetX = npc.X + RandomManager.Next(-3, 4);
                npc.PatrolTargetY = npc.Y + RandomManager.Next(-3, 4);
                npc.PatrolTargetX = Math.Clamp(npc.PatrolTargetX, 0, LocalMap.Width - 1);
                npc.PatrolTargetY = Math.Clamp(npc.PatrolTargetY, 0, LocalMap.Height - 1);
            }

            if (CanNPCStep(newX, newY, map))
            {
                npc.X = newX;
                npc.Y = newY;
            }
        }
    }

    private bool CanNPCStep(int x, int y, LocalMap map)
    {
        if (x < 0 || x >= LocalMap.Width || y < 0 || y >= LocalMap.Height)
            return false;

        var cell = map.GetCell(x, y);
        if (cell == null || (cell.Terrain != TerrainType.Empty && cell.Terrain != TerrainType.Nebula))
            return false;

        if (map.GetNpcAt(x, y) != null)
            return false;

        if (_player.X == x && _player.Y == y)
            return false;

        return true;
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}