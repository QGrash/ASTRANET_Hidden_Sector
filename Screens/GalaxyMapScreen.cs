// Screens/GalaxyMapScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Combat;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Prototypes;
using ASTRANET.Models.World;
using ASTRANET.Utils;

namespace ASTRANET.Screens;

public class GalaxyMapScreen : BaseScreen
{
    private readonly WorldManager _worldManager;
    private readonly Player _player;
    private int _selectedSystemIndex = 0;
    private readonly List<(string text, ConsoleColor color)> _hints;
    private Star[] _stars;
    private const int STAR_COUNT = 100;

    public GalaxyMapScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _worldManager = DI.Resolve<WorldManager>();
        _player = DI.Resolve<Player>();
        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓/←/→ - выбор", ConsoleColor.Gray),
            ("Enter - прыжок (10 топлива)", ConsoleColor.Gray),
            ("F - SOS (20 энергии)", ConsoleColor.Gray),
            ("Backspace - назад в главное меню", ConsoleColor.Gray)
        };
        GenerateStars();

        if (_worldManager.Galaxy.Systems.Count > 0)
        {
            _selectedSystemIndex = 0;
            _worldManager.TryEnterSystem(0, 0);
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (_worldManager.Galaxy == null || _worldManager.Galaxy.Systems.Count == 0)
        {
            _worldManager.GenerateGalaxy(12345);
        }
        UIManager.ForceFullRedraw();
    }

    private void GenerateStars()
    {
        _stars = new Star[STAR_COUNT];
        for (int i = 0; i < STAR_COUNT; i++)
        {
            _stars[i] = new Star
            {
                X = RandomManager.Next(UIManager.Width),
                Y = RandomManager.Next(UIManager.Height - 4),
                Brightness = RandomManager.Next(4)
            };
        }
    }

    public override void Update(float deltaTime)
    {
        if (RandomManager.NextDouble() < 0.01)
        {
            int idx = RandomManager.Next(_stars.Length);
            _stars[idx].Brightness = (_stars[idx].Brightness + 1) % 4;
        }
    }

    public override void Render()
    {
        DrawStars();
        DrawShipStatus();

        var galaxy = _worldManager.Galaxy;
        if (galaxy == null || galaxy.Systems.Count == 0)
        {
            UIManager.DrawString(2, 5, "Ошибка: галактика не загружена!", ConsoleColor.Red);
            UIManager.DrawHints(_hints);
            return;
        }

        DrawConnections();

        foreach (var system in galaxy.Systems)
        {
            // Проверяем, должен ли быть виден скрытый сектор Зета
            bool isZeta = system.Id == 12; // индекс Zeta
            if (system.Hidden && !system.Visited)
            {
                if (isZeta && _player.Inventory.HasItem("fragment_coord_zeta", 3))
                {
                    // Можно показать, но не менять свойство постоянно
                }
                else
                {
                    continue;
                }
            }

            char symbol = system.Type switch
            {
                SystemType.Habitable => 'O',
                SystemType.Desert => 'O',
                SystemType.Hostile => 'O',
                SystemType.Anomaly => '~',
                SystemType.Hidden => '?',
                _ => 'O'
            };
            ConsoleColor color = system.Type switch
            {
                SystemType.Habitable => ConsoleColor.Green,
                SystemType.Desert => ConsoleColor.Yellow,
                SystemType.Hostile => ConsoleColor.Red,
                SystemType.Anomaly => ConsoleColor.Magenta,
                SystemType.Hidden => ConsoleColor.DarkGray,
                _ => ConsoleColor.Gray
            };

            bool isCurrent = _worldManager.CurrentSystem != null && _worldManager.CurrentSystem.Id == system.Id;
            bool isSelected = system.Id == _selectedSystemIndex;

            if (isCurrent)
            {
                UIManager.DrawString(system.X - 1, system.Y, "[", ConsoleColor.Green);
                UIManager.SetPixel(system.X, system.Y, symbol, color);
                UIManager.DrawString(system.X + 1, system.Y, "]", ConsoleColor.Green);
            }
            else if (isSelected)
            {
                UIManager.DrawString(system.X - 1, system.Y, "[", ConsoleColor.White);
                UIManager.SetPixel(system.X, system.Y, symbol, color);
                UIManager.DrawString(system.X + 1, system.Y, "]", ConsoleColor.White);
            }
            else
            {
                UIManager.SetPixel(system.X, system.Y, symbol, color);
            }
        }

        if (_selectedSystemIndex >= 0 && _selectedSystemIndex < galaxy.Systems.Count)
        {
            var selected = galaxy.Systems[_selectedSystemIndex];
            int infoX = UIManager.Width - 35;
            int infoY = 2;
            UIManager.DrawString(infoX, infoY, $"Система: {selected.Name}", ConsoleColor.Yellow);
            UIManager.DrawString(infoX, infoY + 1, $"Тип: {selected.Type}", ConsoleColor.Cyan);
            UIManager.DrawString(infoX, infoY + 2, $"Посещена: {(selected.Visited ? "да" : "нет")}", ConsoleColor.Gray);
            string connections = string.Join(", ", selected.ConnectedSystems.Select(id => galaxy.Systems.FirstOrDefault(s => s.Id == id)?.Name ?? id.ToString()));
            UIManager.DrawString(infoX, infoY + 3, $"Связи: {connections}", ConsoleColor.Gray);
        }

        UIManager.DrawHints(_hints);
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

    private void DrawStars()
    {
        foreach (var star in _stars)
        {
            char c = star.Brightness switch
            {
                0 => '.',
                1 => '.',
                2 => '·',
                _ => '·'
            };
            ConsoleColor color = star.Brightness switch
            {
                0 => ConsoleColor.DarkGray,
                1 => ConsoleColor.Gray,
                2 => ConsoleColor.White,
                _ => ConsoleColor.White
            };
            if (UIManager.GetPixel(star.X, star.Y) == ' ')
                UIManager.SetPixel(star.X, star.Y, c, color);
        }
    }

    private void DrawConnections()
    {
        var galaxy = _worldManager.Galaxy;
        if (galaxy == null) return;

        foreach (var system in galaxy.Systems)
        {
            foreach (var targetId in system.ConnectedSystems)
            {
                var target = galaxy.Systems.FirstOrDefault(s => s.Id == targetId);
                if (target == null) continue;

                if (system.Hidden && !system.Visited) continue;
                if (target.Hidden && !target.Visited) continue;

                DrawLine(system.X, system.Y, target.X, target.Y, ConsoleColor.Cyan);
            }
        }
    }

    private void DrawLine(int x1, int y1, int x2, int y2, ConsoleColor color)
    {
        int dx = Math.Abs(x2 - x1), dy = Math.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x1 >= 0 && x1 < UIManager.Width && y1 >= 0 && y1 < UIManager.Height)
            {
                if (UIManager.GetPixel(x1, y1) == ' ')
                    UIManager.SetPixel(x1, y1, '·', color);
            }
            if (x1 == x2 && y1 == y2) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x1 += sx; }
            if (e2 < dx) { err += dx; y1 += sy; }
        }
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        var galaxy = _worldManager.Galaxy;
        if (galaxy == null || galaxy.Systems.Count == 0) return false;

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.LeftArrow:
                _selectedSystemIndex = (_selectedSystemIndex - 1 + galaxy.Systems.Count) % galaxy.Systems.Count;
                return true;
            case ConsoleKey.DownArrow:
            case ConsoleKey.RightArrow:
                _selectedSystemIndex = (_selectedSystemIndex + 1) % galaxy.Systems.Count;
                return true;
            case ConsoleKey.Enter:
                // Особая обработка для Zeta (индекс 12)
                if (_selectedSystemIndex == 12)
                {
                    // Проверяем, есть ли все фрагменты
                    if (_player.Inventory.HasItem("fragment_coord_zeta", 3))
                    {
                        StartBossFight();
                        return true;
                    }
                    else
                    {
                        UIManager.ShowMessage("Сектор Зета заблокирован. Нужны три фрагмента координат.");
                        return true;
                    }
                }

                if (_worldManager.CurrentSystem != null && _worldManager.CurrentSystem.Id == _selectedSystemIndex)
                {
                    _worldManager.TryEnterSystem(_selectedSystemIndex, 0);
                    var localMapScreen = new LocalMapScreen(UIManager, ScreenManager, InputManager);
                    ScreenManager.PushScreen(localMapScreen);
                    return true;
                }

                if (_worldManager.CurrentSystem != null &&
                    !_worldManager.CurrentSystem.ConnectedSystems.Contains(_selectedSystemIndex))
                {
                    UIManager.ShowMessage("Нет гиперпространственной связи с этой системой!");
                    return true;
                }

                if (_player.Fuel < 10)
                {
                    UIManager.ShowMessage("Недостаточно топлива! Нужно 10 единиц.");
                    return true;
                }

                if (_player.ConsumeFuel(10))
                {
                    var ftlScreen = new FTLScreen(UIManager, ScreenManager, InputManager, _selectedSystemIndex);
                    ScreenManager.PushScreen(ftlScreen);
                }
                return true;
            case ConsoleKey.Backspace:
                var mainMenu = new MainMenuScreen(UIManager, ScreenManager, InputManager);
                ScreenManager.ChangeScreen(mainMenu);
                return true;
            default:
                return false;
        }
    }

    private void StartBossFight()
    {
        // Создаём босса
        var boss = new SpaceShip(
            "ancient_dreadnought",
            "Древний дредноут",
            FactionId.CultOfVoid,
            500,
            300,
            200
        );

        // Добавляем оружие (можно загружать из JSON, но для простоты создаём в коде)
        var weapon1 = new SpaceWeapon(new ItemPrototype
        {
            Id = "ancient_plasma",
            Name = "Древняя плазма",
            DamageMin = 40,
            DamageMax = 60,
            Accuracy = 5,
            EnergyCost = 30,
            ShotsPerTurn = 2,
            CritChance = 0.2,
            CritMultiplier = 2.0,
            DamageType = DamageType.Plasma
        });
        boss.Weapons.Add(weapon1);

        var weapon2 = new SpaceWeapon(new ItemPrototype
        {
            Id = "ancient_ion",
            Name = "Ионный луч",
            DamageMin = 15,
            DamageMax = 25,
            Accuracy = 15,
            EnergyCost = 15,
            ShotsPerTurn = 1,
            CritChance = 0.1,
            CritMultiplier = 1.5,
            DamageType = DamageType.Ion
        });
        boss.Weapons.Add(weapon2);

        var enemies = new List<SpaceShip> { boss };
        var combatManager = new SpaceCombatManager(_player, enemies, _worldManager);
        combatManager.StartCombat();

        var combatScreen = new SpaceCombatScreen(UIManager, ScreenManager, InputManager, combatManager);
        ScreenManager.PushScreen(combatScreen);
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;

    private struct Star
    {
        public int X;
        public int Y;
        public int Brightness;
    }
}