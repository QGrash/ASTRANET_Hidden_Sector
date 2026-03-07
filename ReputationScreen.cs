using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Faction;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class ReputationScreen : GameScreen
    {
        private List<Faction> factions;
        private int selectedIndex = 0;

        public ReputationScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
            factions = FactionManager.GetAllFactions();
        }

        public override void Render()
        {
            uiManager.Clear();

            int leftX = 5;
            int topY = 3;

            // Заголовок
            uiManager.SetCursorPosition(leftX, topY - 1);
            uiManager.Write("=== РЕПУТАЦИЯ И ФРАКЦИИ ===", ConsoleColor.Yellow);

            // Список фракций
            int listStartY = topY + 1;
            for (int i = 0; i < factions.Count; i++)
            {
                var faction = factions[i];
                int rep = FactionManager.GetReputation(faction.Id);
                var level = FactionManager.GetReputationLevel(faction.Id);

                // Выбор цвета для уровня
                ConsoleColor levelColor = level switch
                {
                    ReputationLevel.Hostile => ConsoleColor.Red,
                    ReputationLevel.Negative => ConsoleColor.DarkRed,
                    ReputationLevel.Neutral => ConsoleColor.Yellow,
                    ReputationLevel.Friendly => ConsoleColor.Green,
                    ReputationLevel.Ally => ConsoleColor.Cyan,
                    _ => ConsoleColor.Gray
                };

                // Выделение выбранной строки
                if (i == selectedIndex)
                {
                    uiManager.SetCursorPosition(leftX - 2, listStartY + i);
                    uiManager.Write(">", ConsoleColor.Yellow);
                }

                uiManager.SetCursorPosition(leftX, listStartY + i);
                uiManager.Write($"{faction.Name} [{rep}]", levelColor);
            }

            // Детали выбранной фракции
            if (selectedIndex >= 0 && selectedIndex < factions.Count)
            {
                var faction = factions[selectedIndex];
                int rep = FactionManager.GetReputation(faction.Id);
                var level = FactionManager.GetReputationLevel(faction.Id);

                int detailsX = 40;
                int detailsY = topY + 1;

                uiManager.SetCursorPosition(detailsX, detailsY++);
                uiManager.Write($"Описание: {faction.Description}", ConsoleColor.Gray);
                uiManager.SetCursorPosition(detailsX, detailsY++);
                uiManager.Write($"Репутация: {rep}", ConsoleColor.White);
                uiManager.SetCursorPosition(detailsX, detailsY++);
                uiManager.Write($"Уровень: {level}", GetLevelColor(level));
            }

            // Подсказки
            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("↑/↓ - выбор, Esc/Backspace - назад", ConsoleColor.DarkGray);
        }

        private ConsoleColor GetLevelColor(ReputationLevel level)
        {
            return level switch
            {
                ReputationLevel.Hostile => ConsoleColor.Red,
                ReputationLevel.Negative => ConsoleColor.DarkRed,
                ReputationLevel.Neutral => ConsoleColor.Yellow,
                ReputationLevel.Friendly => ConsoleColor.Green,
                ReputationLevel.Ally => ConsoleColor.Cyan,
                _ => ConsoleColor.Gray
            };
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + factions.Count) % factions.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % factions.Count;
                    break;
                case ConsoleKey.Escape:
                case ConsoleKey.Backspace:
                    stateManager.PopScreen();
                    break;
            }
        }
    }
}