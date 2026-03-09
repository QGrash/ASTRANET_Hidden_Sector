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

            // Сообщение
            for (int i = 0; i < message.Length; i++)
                uiManager.SetPixel(startX + 2 + i, startY + 2, message[i], ConsoleColor.White);

            // Опции
            int optionX = startX + boxWidth / 2 - 4;
            for (int i = 0; i < options.Length; i++)
            {
                string opt = options[i];
                if (i == selectedIndex)
                    opt = "> " + opt + " <";
                else
                    opt = "  " + opt + "  ";
                for (int j = 0; j < opt.Length; j++)
                    uiManager.SetPixel(optionX + j, startY + 4, opt[j],
                        i == selectedIndex ? ConsoleColor.Yellow : ConsoleColor.DarkGray);
                optionX += opt.Length + 2; // небольшой отступ
            }

            string hint = "←/→ для выбора, Enter для подтверждения, Esc - отмена";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
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