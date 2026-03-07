using System;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class ConfirmScreen : GameScreen
    {
        private string message;
        private Action<bool> callback;
        private int selectedIndex = 0;
        private string[] options = { "Да", "Нет" };

        public ConfirmScreen(GameStateManager stateManager, UIManager uiManager, string message, Action<bool> callback)
            : base(stateManager, uiManager)
        {
            this.message = message;
            this.callback = callback;
        }

        public override void Render()
        {
            uiManager.Clear();

            int boxWidth = 60;
            int boxHeight = 8;
            int startX = (Console.WindowWidth - boxWidth) / 2;
            int startY = (Console.WindowHeight - boxHeight) / 2;

            // Рамка
            for (int x = startX; x <= startX + boxWidth; x++)
            {
                uiManager.SetCursorPosition(x, startY);
                uiManager.Write("─", ConsoleColor.Gray);
                uiManager.SetCursorPosition(x, startY + boxHeight);
                uiManager.Write("─", ConsoleColor.Gray);
            }
            for (int y = startY; y <= startY + boxHeight; y++)
            {
                uiManager.SetCursorPosition(startX, y);
                uiManager.Write("│", ConsoleColor.Gray);
                uiManager.SetCursorPosition(startX + boxWidth, y);
                uiManager.Write("│", ConsoleColor.Gray);
            }
            uiManager.SetCursorPosition(startX, startY);
            uiManager.Write("┌", ConsoleColor.Gray);
            uiManager.SetCursorPosition(startX + boxWidth, startY);
            uiManager.Write("┐", ConsoleColor.Gray);
            uiManager.SetCursorPosition(startX, startY + boxHeight);
            uiManager.Write("└", ConsoleColor.Gray);
            uiManager.SetCursorPosition(startX + boxWidth, startY + boxHeight);
            uiManager.Write("┘", ConsoleColor.Gray);

            // Сообщение
            uiManager.SetCursorPosition(startX + 2, startY + 2);
            uiManager.Write(message, ConsoleColor.White);

            // Опции
            int optionX = startX + boxWidth / 2 - 4;
            for (int i = 0; i < options.Length; i++)
            {
                uiManager.SetCursorPosition(optionX + i * 8, startY + 4);
                if (i == selectedIndex)
                    uiManager.Write("> " + options[i] + " <", ConsoleColor.Yellow);
                else
                    uiManager.Write("  " + options[i] + "  ", ConsoleColor.DarkGray);
            }

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("←/→ для выбора, Enter для подтверждения", ConsoleColor.DarkGray);
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    selectedIndex = 0;
                    break;
                case ConsoleKey.RightArrow:
                    selectedIndex = 1;
                    break;
                case ConsoleKey.Enter:
                    callback?.Invoke(selectedIndex == 0);
                    stateManager.PopScreen();
                    break;
                case ConsoleKey.Escape:
                    callback?.Invoke(false);
                    stateManager.PopScreen();
                    break;
            }
        }
    }
}