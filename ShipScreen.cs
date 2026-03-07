using System;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class ShipScreen : GameScreen
    {
        public ShipScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
        }

        public override void Render()
        {
            uiManager.Clear();
            uiManager.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2);
            uiManager.Write("=== КОРАБЛЬ ===", ConsoleColor.Magenta);

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