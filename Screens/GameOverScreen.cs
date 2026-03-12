// Screens/GameOverScreen.cs
using ASTRANET.Core;
using ASTRANET.Managers;
using System;
using System.Collections.Generic;

namespace ASTRANET.Screens;

public class GameOverScreen : BaseScreen
{
    private readonly List<(string text, ConsoleColor color)> _hints;

    public GameOverScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _hints = new List<(string, ConsoleColor)>
        {
            ("Enter - вернуться в главное меню", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        int centerX = UIManager.Width / 2;
        int centerY = UIManager.Height / 2;

        UIManager.DrawString(centerX - 5, centerY - 2, "ИГРА ОКОНЧЕНА", ConsoleColor.Red);
        UIManager.DrawString(centerX - 10, centerY, "Ваш корабль уничтожен...", ConsoleColor.DarkRed);

        UIManager.DrawHints(_hints);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            var mainMenu = new MainMenuScreen(UIManager, ScreenManager, InputManager);
            ScreenManager.ChangeScreen(mainMenu);
            return true;
        }
        return false;
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}