// Screens/CharacterCreationScreen.cs
using System;
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;

namespace ASTRANET.Screens;

public class CharacterCreationScreen : BaseScreen
{
    private readonly List<(PlayerClass @class, string name, string desc)> _classes = new()
    {
        (PlayerClass.Soldier, "Солдат", "+2 Сила, +1 Ловкость. -10% к попаданию по вам."),
        (PlayerClass.CombatEngineer, "Боевой техник", "+2 Интеллект, +1 Ловкость. +25% урона от взрывчатки."),
        (PlayerClass.Diplomat, "Дипломат", "+2 Харизма, +1 Удача. +20% убеждения, шанс избежать боя."),
        (PlayerClass.Scout, "Разведчик", "+2 Ловкость, +1 Интеллект. Скрытность, видит скрытые системы."),
        (PlayerClass.Engineer, "Бортинженер", "+2 Интеллект, +1 Сила. -20% стоимость улучшений, -10% энергопотребление."),
        (PlayerClass.Medic, "Полевой фельдшер", "+2 Интеллект, +1 Харизма. Лечение на 50% эффективнее."),
        (PlayerClass.Quartermaster, "Интендант", "+2 Харизма, +1 Интеллект. -15% цены, +20% доход."),
        (PlayerClass.Cartographer, "Картограф", "+2 Интеллект, +1 Удача. -25% расхода топлива.")
    };

    private int _selectedIndex = 0;
    private string _playerName = "";
    private bool _enteringName = true;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public CharacterCreationScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _hints = new List<(string, ConsoleColor)>
        {
            ("Введите имя (Enter для подтверждения)", ConsoleColor.Gray),
            ("↑/↓ - выбор класса", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        int centerX = UIManager.Width / 2;
        int y = 2;

        UIManager.DrawString(centerX - 10, y++, "СОЗДАНИЕ ПЕРСОНАЖА", ConsoleColor.Yellow);
        y++;

        if (_enteringName)
        {
            UIManager.DrawString(2, y, "Введите имя капитана:", ConsoleColor.Cyan);
            y++;
            UIManager.DrawString(2, y, _playerName + "_", ConsoleColor.White);
            y += 2;
            UIManager.DrawString(2, y, "Нажмите Enter, когда закончите.", ConsoleColor.Gray);
        }
        else
        {
            UIManager.DrawString(centerX - 10, y++, "ВЫБОР КЛАССА", ConsoleColor.Yellow);
            y++;

            for (int i = 0; i < _classes.Count; i++)
            {
                var (_, name, desc) = _classes[i];
                if (i == _selectedIndex)
                {
                    UIManager.DrawString(centerX - 15, y, "> " + name, ConsoleColor.Yellow);
                    UIManager.DrawString(centerX - 13, y + 1, desc, ConsoleColor.Cyan);
                    y += 3;
                }
                else
                {
                    UIManager.DrawString(centerX - 13, y, name, ConsoleColor.White);
                    y++;
                }
            }

            UIManager.DrawString(2, UIManager.Height - 4, $"Имя: {_playerName}", ConsoleColor.Green);
        }

        UIManager.DrawHints(_hints);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (_enteringName)
        {
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (string.IsNullOrWhiteSpace(_playerName))
                    _playerName = "Капитан";
                _enteringName = false;
                return true;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && _playerName.Length > 0)
            {
                _playerName = _playerName.Substring(0, _playerName.Length - 1);
                return true;
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                _playerName += keyInfo.KeyChar;
                return true;
            }
            return true;
        }
        else
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    _selectedIndex = (_selectedIndex - 1 + _classes.Count) % _classes.Count;
                    return true;
                case ConsoleKey.DownArrow:
                    _selectedIndex = (_selectedIndex + 1) % _classes.Count;
                    return true;
                case ConsoleKey.Enter:
                    var player = new Player();
                    player.Name = _playerName;
                    player.Class = _classes[_selectedIndex].@class;
                    player.ApplyClassBonuses();

                    var itemManager = DI.Resolve<ItemManager>();
                    var weapon = itemManager.CreateInstance("laser_pistol");
                    player.Inventory.AddItem(weapon);
                    player.Weapon = weapon;
                    player.Inventory.AddItem(itemManager.CreateInstance("medkit_basic", 3));

                    DI.Register(player);

                    var galaxyScreen = new GalaxyMapScreen(UIManager, ScreenManager, InputManager);
                    ScreenManager.ChangeScreen(galaxyScreen);
                    return true;
                default:
                    return false;
            }
        }
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}