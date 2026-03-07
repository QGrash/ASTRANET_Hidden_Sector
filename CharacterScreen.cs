using System;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class CharacterScreen : GameScreen
    {
        public CharacterScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
        }

        public override void Render()
        {
            uiManager.Clear();
            uiManager.SetCursorPosition(Console.WindowWidth / 2 - 12, Console.WindowHeight / 2);
            uiManager.Write("=== ХАРАКТЕРИСТИКИ ===", ConsoleColor.Green);

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("Backspace - назад, ESC - меню", ConsoleColor.DarkGray);
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Backspace)
            {
                stateManager.PopScreen();
            }
        }
    }
}