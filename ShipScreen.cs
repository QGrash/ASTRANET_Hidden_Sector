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

            int leftX = 5;
            int topY = 3;

            string title = "=== КОРАБЛЬ ===";
            for (int i = 0; i < title.Length; i++)
                uiManager.SetPixel(leftX + i, topY, title[i], ConsoleColor.Magenta);
            topY += 2;

            // Информация о корабле (заглушка, позже будет реальная)
            string[] info = new string[]
            {
                "Название: Фрегат класса 'Вольный'",
                "Корпус: 100/100",
                "Щиты: 50/50 (реген 5/ход)",
                "Энергия: 50/50",
                "Двигатели: мощность 10, уклонение 10%",
                "Установленные модули:",
                "  - Лазерная пушка (урон 10-15)",
                "  - Плазменная пушка (урон 8-12)",
                "Грузовой отсек: 10/50 кг"
            };

            foreach (string line in info)
            {
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, line[i], ConsoleColor.White);
                topY++;
            }

            string hint = "Backspace - назад, ESC - меню";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
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