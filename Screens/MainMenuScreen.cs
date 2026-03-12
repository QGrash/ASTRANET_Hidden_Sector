// Screens/MainMenuScreen.cs
using System;
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Managers;

namespace ASTRANET.Screens;

public class MainMenuScreen : BaseScreen
{
    private readonly string[] _menuItems = { "Новая игра", "Загрузить", "Выход" };
    private int _selectedIndex = 0;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public MainMenuScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓ - выбор", ConsoleColor.Gray),
            ("Enter - подтвердить", ConsoleColor.Gray),
            ("ESC - выход", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        int centerX = UIManager.Width / 2;
        int centerY = UIManager.Height / 3;

        string title = "ASTRANET: Hidden Sector";
        UIManager.DrawString(centerX - title.Length / 2, centerY - 4, title, ConsoleColor.Cyan);

        for (int i = 0; i < _menuItems.Length; i++)
        {
            string item = _menuItems[i];
            if (i == _selectedIndex)
            {
                item = "> " + item + " <";
                UIManager.DrawString(centerX - item.Length / 2, centerY + i * 2, item, ConsoleColor.Yellow);
            }
            else
            {
                UIManager.DrawString(centerX - item.Length / 2, centerY + i * 2, item, ConsoleColor.White);
            }
        }

        UIManager.DrawHints(_hints);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                _selectedIndex = (_selectedIndex - 1 + _menuItems.Length) % _menuItems.Length;
                return true;
            case ConsoleKey.DownArrow:
                _selectedIndex = (_selectedIndex + 1) % _menuItems.Length;
                return true;
            case ConsoleKey.Enter:
                switch (_selectedIndex)
                {
                    case 0:
                        var charCreationScreen = new CharacterCreationScreen(UIManager, ScreenManager, InputManager);
                        ScreenManager.ChangeScreen(charCreationScreen);
                        break;
                    case 1:
                        var loadScreen = new LoadGameScreen(UIManager, ScreenManager, InputManager);
                        ScreenManager.ChangeScreen(loadScreen);
                        break;
                    case 2:
                        Environment.Exit(0);
                        break;
                }
                return true;
            default:
                return false;
        }
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}