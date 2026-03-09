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
            int startX = 2;
            int startY = 3;

            // Рамка
            for (int x = startX; x <= startX + boxWidth; x++)
            {
                uiManager.SetPixel(x, startY, '─', ConsoleColor.Gray);
                uiManager.SetPixel(x, startY + boxHeight, '─', ConsoleColor.Gray);
            }
            for (int y = startY; y <= startY + boxHeight; y++)
            {
                uiManager.SetPixel(startX, y, '│', ConsoleColor.Gray);
                uiManager.SetPixel(startX + boxWidth, y, '│', ConsoleColor.Gray);
            }
            uiManager.SetPixel(startX, startY, '┌', ConsoleColor.Gray);
            uiManager.SetPixel(startX + boxWidth, startY, '┐', ConsoleColor.Gray);
            uiManager.SetPixel(startX, startY + boxHeight, '└', ConsoleColor.Gray);
            uiManager.SetPixel(startX + boxWidth, startY + boxHeight, '┘', ConsoleColor.Gray);

            // Заголовок
            for (int i = 0; i < title.Length; i++)
                uiManager.SetPixel(startX + 2 + i, startY + 1, title[i], ConsoleColor.Yellow);

            // Разделитель
            for (int i = 0; i < boxWidth - 4; i++)
                uiManager.SetPixel(startX + 2 + i, startY + 2, '─', ConsoleColor.DarkGray);

            // Текст сообщения
            int textY = startY + 3;
            foreach (var line in lines)
            {
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(startX + 2 + i, textY, line[i], ConsoleColor.White);
                textY++;
            }

            // Подсказка
            string hint = "Нажмите любую клавишу для продолжения...";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(startX + 2 + i, startY + boxHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            stateManager.PopScreen();
        }
    }
}