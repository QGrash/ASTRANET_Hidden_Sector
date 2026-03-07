using System;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class MainMenuScreen : GameScreen
    {
        private string[] menuItems = { "Новая игра", "Загрузка", "Выход" };
        private int selectedIndex = 0;

        public MainMenuScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
        }

        public override void Render()
        {
            uiManager.Clear(); // очищает экран через буфер

            int centerX = Console.WindowWidth / 2;
            int startY = Console.WindowHeight / 2 - menuItems.Length;

            for (int i = 0; i < menuItems.Length; i++)
            {
                // Устанавливаем позицию для каждой строки
                uiManager.SetCursorPosition(centerX - menuItems[i].Length / 2, startY + i);
                if (i == selectedIndex)
                    uiManager.Write(menuItems[i], ConsoleColor.Yellow);
                else
                    uiManager.Write(menuItems[i], ConsoleColor.Gray);
            }

            // Подсказка
            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("↑/↓ для выбора, Enter для подтверждения", ConsoleColor.DarkGray);

            // После всех записей вызываем Render для вывода на экран
            // (это делает Game.Run, поэтому здесь не нужно)
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % menuItems.Length;
                    break;
                case ConsoleKey.Enter:
                    ExecuteMenuItem();
                    break;
            }
        }

        private void ExecuteMenuItem()
        {
            switch (selectedIndex)
            {
                case 0: // Новая игра
                    stateManager.ChangeScreen(new CharacterCreationScreen(stateManager, uiManager));
                    break;
                case 1: // Загрузка
                    stateManager.PushScreen(new LoadGameMenuScreen(stateManager, uiManager));
                    break;
                case 2: // Выход
                    Environment.Exit(0);
                    break;
            }
        }
    }
}