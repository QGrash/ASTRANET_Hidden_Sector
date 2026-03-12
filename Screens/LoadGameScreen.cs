// Screens/LoadGameScreen.cs
using System;
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Managers;

namespace ASTRANET.Screens;

public class LoadGameScreen : BaseScreen
{
    private readonly SaveLoadManager _saveLoad;
    private List<string> _saveFiles;
    private int _selectedIndex = 0;
    private int _scrollOffset = 0;
    private const int ItemsPerPage = 10;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public LoadGameScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _saveLoad = new SaveLoadManager();
        _saveFiles = _saveLoad.GetSaveFiles();
        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓ - выбор файла", ConsoleColor.Gray),
            ("Enter - загрузить", ConsoleColor.Gray),
            ("Backspace - назад", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        UIManager.DrawString(2, 2, "ЗАГРУЗКА ИГРЫ", ConsoleColor.Yellow);
        UIManager.DrawString(2, 3, new string('=', 30), ConsoleColor.DarkGray);

        if (_saveFiles.Count == 0)
        {
            UIManager.DrawString(2, 5, "Нет сохранённых игр.", ConsoleColor.Gray);
        }
        else
        {
            int startY = 5;
            for (int i = 0; i < ItemsPerPage; i++)
            {
                int idx = _scrollOffset + i;
                if (idx >= _saveFiles.Count) break;

                string file = _saveFiles[idx];
                if (idx == _selectedIndex)
                    UIManager.DrawString(2, startY + i, "> " + file, ConsoleColor.Yellow);
                else
                    UIManager.DrawString(2, startY + i, "  " + file, ConsoleColor.White);
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
                if (_selectedIndex < _saveFiles.Count - 1)
                {
                    _selectedIndex++;
                    if (_selectedIndex >= _scrollOffset + ItemsPerPage)
                        _scrollOffset = _selectedIndex - ItemsPerPage + 1;
                }
                return true;
            case ConsoleKey.Enter:
                if (_saveFiles.Count > 0)
                {
                    if (_saveLoad.LoadGame(_saveFiles[_selectedIndex]))
                    {
                        var galaxyScreen = new GalaxyMapScreen(UIManager, ScreenManager, InputManager);
                        ScreenManager.ChangeScreen(galaxyScreen);
                    }
                    else
                    {
                        UIManager.ShowMessage("Ошибка загрузки!");
                    }
                }
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