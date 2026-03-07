using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class MessageScreen : GameScreen
    {
        private string title;
        private string message;
        private string[] lines;

        public MessageScreen(GameStateManager stateManager, UIManager uiManager, string title, string message)
            : base(stateManager, uiManager)
        {
            this.title = title;
            this.message = message;
            lines = WrapText(message, 60);
        }

        private string[] WrapText(string text, int maxWidth)
        {
            var words = text.Split(' ');
            var result = new List<string>();
            var currentLine = "";

            foreach (var word in words)
            {
                if ((currentLine + " " + word).Trim().Length <= maxWidth)
                {
                    if (currentLine.Length > 0)
                        currentLine += " ";
                    currentLine += word;
                }
                else
                {
                    if (currentLine.Length > 0)
                        result.Add(currentLine);
                    currentLine = word;
                }
            }
            if (currentLine.Length > 0)
                result.Add(currentLine);

            return result.ToArray();
        }

        public override void Render()
        {
            uiManager.Clear();

            int boxWidth = 64;
            int boxHeight = lines.Length + 6;
            int startX = (Console.WindowWidth - boxWidth) / 2;
            int startY = (Console.WindowHeight - boxHeight) / 2;

            uiManager.SetCursorPosition(startX, startY);
            uiManager.Write("┌" + new string('─', boxWidth - 2) + "┐", ConsoleColor.Gray);

            for (int i = 1; i <= boxHeight - 2; i++)
            {
                uiManager.SetCursorPosition(startX, startY + i);
                uiManager.Write("│", ConsoleColor.Gray);
                uiManager.SetCursorPosition(startX + boxWidth - 1, startY + i);
                uiManager.Write("│", ConsoleColor.Gray);
            }

            uiManager.SetCursorPosition(startX, startY + boxHeight - 1);
            uiManager.Write("└" + new string('─', boxWidth - 2) + "┘", ConsoleColor.Gray);

            uiManager.SetCursorPosition(startX + 2, startY + 1);
            uiManager.Write(title, ConsoleColor.Yellow);

            uiManager.SetCursorPosition(startX + 2, startY + 2);
            uiManager.Write(new string('─', boxWidth - 4), ConsoleColor.DarkGray);

            for (int i = 0; i < lines.Length; i++)
            {
                uiManager.SetCursorPosition(startX + 2, startY + 3 + i);
                uiManager.Write(lines[i], ConsoleColor.White);
            }

            uiManager.SetCursorPosition(startX + 2, startY + boxHeight - 2);
            uiManager.Write("Нажмите любую клавишу для продолжения...", ConsoleColor.DarkGray);
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            stateManager.PopScreen(); // любая клавиша закрывает
        }
    }
}