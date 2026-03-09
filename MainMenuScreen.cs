using System;
using ASTRANET_Hidden_Sector.Core;

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
            uiManager.Clear();

            int startX = 2;
            int startY = 2;

            for (int i = 0; i < menuItems.Length; i++)
            {
                string text = menuItems[i];
                int y = startY + i;
                if (i == selectedIndex)
                {
                    uiManager.SetPixel(startX, y, '>', ConsoleColor.Yellow);
                    for (int j = 0; j < text.Length; j++)
                        uiManager.SetPixel(startX + 2 + j, y, text[j], ConsoleColor.Yellow);
                }
                else
                {
                    uiManager.SetPixel(startX, y, ' ', ConsoleColor.Black);
                    for (int j = 0; j < text.Length; j++)
                        uiManager.SetPixel(startX + 2 + j, y, text[j], ConsoleColor.Gray);
                }
            }

            string hint = "↑/↓ для выбора, Enter для подтверждения";
            int hintY = Console.WindowHeight - 2;
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(startX + i, hintY, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0)
                        selectedIndex = menuItems.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= menuItems.Length)
                        selectedIndex = 0;
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
                case 0:
                    stateManager.ChangeScreen(new CharacterCreationScreen(stateManager, uiManager));
                    break;
                case 1:
                    stateManager.PushScreen(new LoadGameMenuScreen(stateManager, uiManager));
                    break;
                case 2:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}