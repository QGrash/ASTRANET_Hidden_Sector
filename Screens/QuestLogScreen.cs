// Screens/QuestLogScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Instances;

namespace ASTRANET.Screens;

public class QuestLogScreen : BaseScreen
{
    private readonly QuestManager _questManager;
    private List<QuestInstance> _activeQuests;
    private int _selectedIndex = 0;
    private int _scrollOffset = 0;
    private const int QuestsPerPage = 5;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public QuestLogScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _questManager = DI.Resolve<QuestManager>();
        RefreshQuests();
        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓ - выбор", ConsoleColor.Gray),
            ("Backspace - назад", ConsoleColor.Gray)
        };
    }

    private void RefreshQuests()
    {
        _activeQuests = _questManager.ActiveQuests.Where(q => q.State == QuestState.Active).ToList();
    }

    public override void Render()
    {
        UIManager.Clear();

        UIManager.DrawString(2, 2, "ЖУРНАЛ КВЕСТОВ", ConsoleColor.Yellow);
        UIManager.DrawString(2, 3, new string('=', 30), ConsoleColor.DarkGray);

        if (_activeQuests.Count == 0)
        {
            UIManager.DrawString(2, 5, "Нет активных квестов.", ConsoleColor.Gray);
        }
        else
        {
            int startY = 5;
            for (int i = 0; i < QuestsPerPage; i++)
            {
                int idx = _scrollOffset + i;
                if (idx >= _activeQuests.Count) break;

                var quest = _activeQuests[idx];
                string line = quest.Prototype?.Name ?? "Неизвестный квест";
                if (idx == _selectedIndex)
                    UIManager.DrawString(2, startY + i, "> " + line, ConsoleColor.Yellow);
                else
                    UIManager.DrawString(2, startY + i, "  " + line, ConsoleColor.White);
            }
        }

        if (_activeQuests.Count > 0 && _selectedIndex >= 0 && _selectedIndex < _activeQuests.Count)
        {
            var quest = _activeQuests[_selectedIndex];
            int detailsY = 5 + QuestsPerPage + 2;
            UIManager.DrawString(2, detailsY, quest.Prototype?.Description ?? "", ConsoleColor.Cyan);
            detailsY += 2;
            for (int i = 0; i < quest.Prototype?.Objectives.Count; i++)
            {
                var obj = quest.Prototype.Objectives[i];
                int progress = quest.ObjectiveProgress[i];
                string status = $"{obj.Description}: {progress}/{obj.Amount}";
                if (progress >= obj.Amount)
                    UIManager.DrawString(2, detailsY + i, status, ConsoleColor.Green);
                else
                    UIManager.DrawString(2, detailsY + i, status, ConsoleColor.Gray);
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
                if (_selectedIndex < _activeQuests.Count - 1)
                {
                    _selectedIndex++;
                    if (_selectedIndex >= _scrollOffset + QuestsPerPage)
                        _scrollOffset = _selectedIndex - QuestsPerPage + 1;
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