// Screens/SpaceCombatScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Combat;
using ASTRANET.Models.Entities;
using ASTRANET.Utils;

namespace ASTRANET.Screens;

public class SpaceCombatScreen : BaseScreen
{
    private SpaceCombatManager _combatManager;
    private readonly List<(string text, ConsoleColor color)> _hints;
    private int _selectedWeaponIndex = 0;
    private int _selectedEnemyIndex = 0;

    public SpaceCombatScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager, SpaceCombatManager combatManager)
        : base(uiManager, inputManager, screenManager)
    {
        _combatManager = combatManager;
        _hints = new List<(string, ConsoleColor)>
        {
            ("←/→ - выбор оружия", ConsoleColor.Gray),
            ("↑/↓ - выбор цели", ConsoleColor.Gray),
            ("Enter - стрелять", ConsoleColor.Gray),
            ("Space - пропустить ход", ConsoleColor.Gray),
            ("F - аварийный прыжок", ConsoleColor.Gray),
            ("Backspace - сдаться (выйти)", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        UIManager.DrawString(2, 2, "КОСМИЧЕСКИЙ БОЙ", ConsoleColor.Red);
        UIManager.DrawString(2, 3, new string('=', 50), ConsoleColor.DarkGray);

        int y = 5;
        UIManager.DrawString(2, y++, "ВАШ КОРАБЛЬ:", ConsoleColor.Yellow);
        UIManager.DrawString(2, y++, $"Корпус: {_combatManager.Player.Hull}/{_combatManager.Player.MaxHull}", ConsoleColor.Red);
        UIManager.DrawString(2, y++, $"Щиты: {_combatManager.Player.Shields}/{_combatManager.Player.MaxShields}", ConsoleColor.Cyan);
        UIManager.DrawString(2, y++, $"Энергия: {_combatManager.Player.Energy}/{_combatManager.Player.MaxEnergy}", ConsoleColor.Yellow);

        var playerWeapons = new List<SpaceWeapon>();
        if (_combatManager.Player.Weapon?.Prototype != null)
        {
            playerWeapons.Add(new SpaceWeapon(_combatManager.Player.Weapon.Prototype));
        }

        if (playerWeapons.Count == 0)
        {
            UIManager.DrawString(2, y++, "Оружие не экипировано!", ConsoleColor.Red);
        }
        else
        {
            UIManager.DrawString(2, y++, "Оружие:", ConsoleColor.Green);
            for (int i = 0; i < playerWeapons.Count; i++)
            {
                string prefix = (i == _selectedWeaponIndex) ? "> " : "  ";
                string line = $"{prefix}{playerWeapons[i].Name} (урон {playerWeapons[i].MinDamage}-{playerWeapons[i].MaxDamage}, эн. {playerWeapons[i].EnergyCost})";
                UIManager.DrawString(2, y + i, line, i == _selectedWeaponIndex ? ConsoleColor.Yellow : ConsoleColor.White);
            }
            y += playerWeapons.Count;
        }

        y += 2;
        UIManager.DrawString(2, y++, "ВРАГИ:", ConsoleColor.Red);
        for (int i = 0; i < _combatManager.Enemies.Count; i++)
        {
            var enemy = _combatManager.Enemies[i];
            string prefix = (i == _selectedEnemyIndex) ? "> " : "  ";
            string line = $"{prefix}{enemy.Name} (корпус {enemy.Hull}/{enemy.MaxHull}, щиты {enemy.Shields.CurrentShield}/{enemy.Shields.MaxShield})";
            UIManager.DrawString(2, y + i, line, i == _selectedEnemyIndex ? ConsoleColor.Yellow : ConsoleColor.White);
        }

        if (!string.IsNullOrEmpty(_combatManager.Message))
        {
            UIManager.DrawString(2, UIManager.Height - 4, _combatManager.Message, ConsoleColor.Cyan);
        }

        UIManager.DrawHints(_hints);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (!_combatManager.CombatActive)
        {
            ScreenManager.PopScreen();
            return true;
        }

        if (!_combatManager.IsPlayerTurn) return true;

        switch (keyInfo.Key)
        {
            case ConsoleKey.LeftArrow:
                if (_selectedWeaponIndex > 0) _selectedWeaponIndex--;
                return true;
            case ConsoleKey.RightArrow:
                var playerWeapons = new List<SpaceWeapon>();
                if (_combatManager.Player.Weapon?.Prototype != null)
                    playerWeapons.Add(new SpaceWeapon(_combatManager.Player.Weapon.Prototype));
                if (_selectedWeaponIndex < playerWeapons.Count - 1) _selectedWeaponIndex++;
                return true;
            case ConsoleKey.UpArrow:
                if (_selectedEnemyIndex > 0) _selectedEnemyIndex--;
                return true;
            case ConsoleKey.DownArrow:
                if (_selectedEnemyIndex < _combatManager.Enemies.Count - 1) _selectedEnemyIndex++;
                return true;
            case ConsoleKey.Enter:
                playerWeapons = new List<SpaceWeapon>();
                if (_combatManager.Player.Weapon?.Prototype != null)
                    playerWeapons.Add(new SpaceWeapon(_combatManager.Player.Weapon.Prototype));
                if (playerWeapons.Count > 0 && _selectedWeaponIndex < playerWeapons.Count && _selectedEnemyIndex < _combatManager.Enemies.Count)
                {
                    _combatManager.PlayerFire(playerWeapons[_selectedWeaponIndex], _combatManager.Enemies[_selectedEnemyIndex]);
                }
                return true;
            case ConsoleKey.Spacebar:
                _combatManager.PlayerSkipTurn();
                return true;
            case ConsoleKey.F:
                _combatManager.ActivateEscape();
                return true;
            case ConsoleKey.Backspace:
                ScreenManager.PopScreen();
                return true;
            default:
                return false;
        }
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}