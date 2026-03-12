// Screens/ReputationScreen.cs
using System;
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Managers;

namespace ASTRANET.Screens;

public class ReputationScreen : BaseScreen
{
    private readonly ReputationManager _reputationManager;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public ReputationScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _reputationManager = DI.Resolve<ReputationManager>();
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

        UIManager.DrawString(x, y++, "РЕПУТАЦИЯ С ФРАКЦИЯМИ", ConsoleColor.Yellow);
        y++;

        foreach (FactionId faction in Enum.GetValues(typeof(FactionId)))
        {
            int value = _reputationManager.GetReputation(faction);
            var level = _reputationManager.GetReputationLevel(faction);
            string levelStr = level switch
            {
                ReputationLevel.Hostile => "Враждебный",
                ReputationLevel.Negative => "Негативный",
                ReputationLevel.Neutral => "Нейтральный",
                ReputationLevel.Friendly => "Дружественный",
                ReputationLevel.Ally => "Союзник",
                _ => "Неизвестно"
            };
            ConsoleColor color = level switch
            {
                ReputationLevel.Hostile => ConsoleColor.Red,
                ReputationLevel.Negative => ConsoleColor.DarkRed,
                ReputationLevel.Neutral => ConsoleColor.Gray,
                ReputationLevel.Friendly => ConsoleColor.Green,
                ReputationLevel.Ally => ConsoleColor.Cyan,
                _ => ConsoleColor.Gray
            };
            string line = $"{faction}: {value} ({levelStr})";
            UIManager.DrawString(x, y++, line, color);
            if (y > UIManager.Height - 5) break;
        }

        UIManager.DrawHints(_hints);
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