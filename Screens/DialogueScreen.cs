// Screens/DialogueScreen.cs
using System;
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Managers;

namespace ASTRANET.Screens;

public class DialogueScreen : BaseScreen
{
    private readonly DialogueManager _dialogueManager;
    private readonly string _npcName;
    private readonly List<(string text, ConsoleColor color)> _hints;
    private int _selectedChoice = 0;

    public DialogueScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager, string npcName)
        : base(uiManager, inputManager, screenManager)
    {
        _dialogueManager = DI.Resolve<DialogueManager>();
        _npcName = npcName;
        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓ - выбор", ConsoleColor.Gray),
            ("Enter - ответить", ConsoleColor.Gray),
            ("Backspace - завершить", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        // Заголовок
        UIManager.DrawString(2, 2, $"Диалог с {_npcName}", ConsoleColor.Yellow);
        UIManager.DrawString(2, 3, new string('=', 50), ConsoleColor.DarkGray);

        // Текст NPC
        if (_dialogueManager.CurrentNode != null)
        {
            int y = 5;
            foreach (string line in WrapText(_dialogueManager.CurrentNode.NpcText, 50))
            {
                UIManager.DrawString(2, y++, line, ConsoleColor.Cyan);
            }

            // Варианты ответов
            y += 2;
            for (int i = 0; i < _dialogueManager.CurrentNode.Choices.Count; i++)
            {
                string choiceText = _dialogueManager.CurrentNode.Choices[i].Text;
                if (i == _selectedChoice)
                    UIManager.DrawString(2, y + i, "> " + choiceText, ConsoleColor.Yellow);
                else
                    UIManager.DrawString(2, y + i, "  " + choiceText, ConsoleColor.White);
            }
        }

        UIManager.DrawHints(_hints);
    }

    private List<string> WrapText(string text, int maxWidth)
    {
        var lines = new List<string>();
        if (string.IsNullOrEmpty(text)) return lines;
        int start = 0;
        while (start < text.Length)
        {
            int len = Math.Min(maxWidth, text.Length - start);
            lines.Add(text.Substring(start, len));
            start += len;
        }
        return lines;
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (_dialogueManager.CurrentNode == null)
        {
            ScreenManager.PopScreen();
            return true;
        }

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (_selectedChoice > 0)
                    _selectedChoice--;
                return true;
            case ConsoleKey.DownArrow:
                if (_selectedChoice < _dialogueManager.CurrentNode.Choices.Count - 1)
                    _selectedChoice++;
                return true;
            case ConsoleKey.Enter:
                _dialogueManager.MakeChoice(_selectedChoice);
                _selectedChoice = 0;
                if (_dialogueManager.CurrentNode == null)
                    ScreenManager.PopScreen();
                return true;
            case ConsoleKey.Backspace:
                _dialogueManager.EndDialogue();
                ScreenManager.PopScreen();
                return true;
            default:
                return false;
        }
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}