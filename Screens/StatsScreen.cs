// Screens/StatsScreen.cs
using System;
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;

namespace ASTRANET.Screens;

public class StatsScreen : BaseScreen
{
    private readonly Player _player;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public StatsScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _player = DI.Resolve<Player>();
        _hints = new List<(string, ConsoleColor)>
        {
            ("Backspace - назад", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        int x = 2;
        int y = 2;

        UIManager.DrawString(x, y++, "ХАРАКТЕРИСТИКИ ПЕРСОНАЖА", ConsoleColor.Yellow);
        y++;

        UIManager.DrawString(x, y++, $"Класс: {GetClassName(_player.Class)}", ConsoleColor.Cyan);
        y++;
        UIManager.DrawString(x, y++, $"Уровень: {_player.Level}", ConsoleColor.Green);
        UIManager.DrawString(x, y++, $"Опыт: {_player.Experience}/{_player.NextLevelExp}", ConsoleColor.Gray);
        y++;
        UIManager.DrawString(x, y++, $"Сила: {_player.Strength}", ConsoleColor.White);
        UIManager.DrawString(x, y++, $"Ловкость: {_player.Dexterity}", ConsoleColor.White);
        UIManager.DrawString(x, y++, $"Интеллект: {_player.Intelligence}", ConsoleColor.White);
        UIManager.DrawString(x, y++, $"Удача: {_player.Luck}", ConsoleColor.White);
        UIManager.DrawString(x, y++, $"Харизма: {_player.Charisma}", ConsoleColor.White);
        y++;
        UIManager.DrawString(x, y++, $"Здоровье: {_player.Health}/{_player.MaxHealth}", ConsoleColor.Red);
        UIManager.DrawString(x, y++, $"Корпус: {_player.Hull}/{_player.MaxHull}", ConsoleColor.Red);
        UIManager.DrawString(x, y++, $"Щиты: {_player.Shields}/{_player.MaxShields}", ConsoleColor.Cyan);
        UIManager.DrawString(x, y++, $"Энергия: {_player.Energy}/{_player.MaxEnergy}", ConsoleColor.Yellow);
        UIManager.DrawString(x, y++, $"Топливо: {_player.Fuel}/{_player.MaxFuel}", ConsoleColor.Green);
        y++;
        UIManager.DrawString(x, y++, $"Кредиты: {_player.Credits}", ConsoleColor.Yellow);
        double weight = _player.GetTotalWeight();
        int maxWeight = _player.GetMaxWeight();
        UIManager.DrawString(x, y++, $"Вес: {weight:F1}/{maxWeight} кг", weight > maxWeight ? ConsoleColor.Red : ConsoleColor.Gray);

        UIManager.DrawHints(_hints);
    }

    private string GetClassName(PlayerClass cls)
    {
        return cls switch
        {
            PlayerClass.Soldier => "Солдат",
            PlayerClass.CombatEngineer => "Боевой техник",
            PlayerClass.Diplomat => "Дипломат",
            PlayerClass.Scout => "Разведчик",
            PlayerClass.Engineer => "Бортинженер",
            PlayerClass.Medic => "Полевой фельдшер",
            PlayerClass.Quartermaster => "Интендант",
            PlayerClass.Cartographer => "Картограф",
            _ => "Неизвестно"
        };
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Backspace)
        {
            ScreenManager.PopScreen();
            return true;
        }
        return false;
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}