// Screens/UpgradeScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;

namespace ASTRANET.Screens;

public class UpgradeScreen : BaseScreen
{
    private readonly Player _player;
    private readonly TechManager _techManager;
    private List<ShipModule> _modules;
    private int _selectedIndex = 0;
    private int _scrollOffset = 0;
    private const int ItemsPerPage = 10;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public UpgradeScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _player = DI.Resolve<Player>();
        _techManager = new TechManager();
        _modules = _player.InstalledModules.ToList();

        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓ - выбор модуля", ConsoleColor.Gray),
            ("Enter - улучшить", ConsoleColor.Gray),
            ("Backspace - назад", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        UIManager.DrawString(2, 2, "УЛУЧШЕНИЕ МОДУЛЕЙ", ConsoleColor.Yellow);
        UIManager.DrawString(2, 3, new string('=', 50), ConsoleColor.DarkGray);
        UIManager.DrawString(2, 4, $"TechPoints: {_player.TechPoints}", ConsoleColor.Cyan);

        int startY = 6;
        for (int i = 0; i < ItemsPerPage; i++)
        {
            int idx = _scrollOffset + i;
            if (idx >= _modules.Count) break;

            var module = _modules[idx];
            string name = module.Item.Prototype?.Name ?? "Неизвестный модуль";
            string line = $"{name} (ур. {module.Level}) | след. ур. стоимость: {module.GetNextUpgradeCost()} TP";
            if (idx == _selectedIndex)
                UIManager.DrawString(2, startY + i, "> " + line, ConsoleColor.Yellow);
            else
                UIManager.DrawString(2, startY + i, "  " + line, ConsoleColor.White);
        }

        if (_modules.Count > 0 && _selectedIndex < _modules.Count)
        {
            var selected = _modules[_selectedIndex];
            int detailsY = startY + ItemsPerPage + 2;
            UIManager.DrawString(2, detailsY, "Характеристики:", ConsoleColor.Gray);
            detailsY++;
            if (selected.Item.Prototype?.DamageMin != null)
            {
                UIManager.DrawString(2, detailsY, $"Урон: {selected.GetCurrentDamageMin()}-{selected.GetCurrentDamageMax()}", ConsoleColor.White);
                detailsY++;
            }
            if (selected.Item.Prototype?.ShieldBonus != null)
            {
                UIManager.DrawString(2, detailsY, $"Щиты: +{selected.GetCurrentShieldBonus()}", ConsoleColor.White);
                detailsY++;
            }
        }

        UIManager.DrawHints(_hints);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (_selectedIndex > 0)
                {
                    _selectedIndex--;
                    if (_selectedIndex < _scrollOffset)
                        _scrollOffset = _selectedIndex;
                }
                return true;
            case ConsoleKey.DownArrow:
                if (_selectedIndex < _modules.Count - 1)
                {
                    _selectedIndex++;
                    if (_selectedIndex >= _scrollOffset + ItemsPerPage)
                        _scrollOffset = _selectedIndex - ItemsPerPage + 1;
                }
                return true;
            case ConsoleKey.Enter:
                UpgradeSelected();
                return true;
            case ConsoleKey.Backspace:
                ScreenManager.PopScreen();
                return true;
            default:
                return false;
        }
    }

    private void UpgradeSelected()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _modules.Count) return;
        var module = _modules[_selectedIndex];
        if (_techManager.CanUpgrade(module))
        {
            _techManager.UpgradeModule(module);
            UIManager.ShowMessage($"Модуль {module.Item.Prototype?.Name} улучшен до {module.Level} уровня");
        }
        else
        {
            UIManager.ShowMessage("Недостаточно TechPoints!");
        }
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}