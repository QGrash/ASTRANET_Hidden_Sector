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

            string header = "=== РЕПУТАЦИЯ И ФРАКЦИИ ===";
            for (int i = 0; i < header.Length; i++)
                uiManager.SetPixel(leftX + i, topY - 1, header[i], ConsoleColor.Yellow);

            for (int i = 0; i < factions.Count; i++)
            {
                var faction = factions[i];
                int rep = FactionManager.GetReputation(faction.Id);
                var level = FactionManager.GetReputationLevel(faction.Id);

                ConsoleColor levelColor = level switch
                {
                    ReputationLevel.Hostile => ConsoleColor.Red,
                    ReputationLevel.Negative => ConsoleColor.DarkRed,
                    ReputationLevel.Neutral => ConsoleColor.Yellow,
                    ReputationLevel.Friendly => ConsoleColor.Green,
                    ReputationLevel.Ally => ConsoleColor.Cyan,
                    _ => ConsoleColor.Gray
                };

                int y = topY + i;
                if (i == selectedIndex)
                    uiManager.SetPixel(leftX, y, '>', ConsoleColor.Yellow);
                else
                    uiManager.SetPixel(leftX, y, ' ', ConsoleColor.Black);

                string line = $"{faction.Name} [{rep}]";
                for (int j = 0; j < line.Length; j++)
                    uiManager.SetPixel(leftX + 2 + j, y, line[j], levelColor);
            }

            if (selectedIndex >= 0 && selectedIndex < factions.Count)
            {
                var faction = factions[selectedIndex];
                int rep = FactionManager.GetReputation(faction.Id);
                var level = FactionManager.GetReputationLevel(faction.Id);

                int detailsX = leftX + 40;
                int detailsY = topY;

                string desc = $"Описание: {faction.Description}";
                for (int i = 0; i < desc.Length; i++)
                    uiManager.SetPixel(detailsX + i, detailsY, desc[i], ConsoleColor.Gray);
                detailsY++;

                string repStr = $"Репутация: {rep}";
                for (int i = 0; i < repStr.Length; i++)
                    uiManager.SetPixel(detailsX + i, detailsY, repStr[i], ConsoleColor.White);
                detailsY++;

                string levelStr = $"Уровень: {level}";
                for (int i = 0; i < levelStr.Length; i++)
                    uiManager.SetPixel(detailsX + i, detailsY, levelStr[i], GetLevelColor(level));
            }

            string hint = "↑/↓ - выбор, Esc/Backspace - назад";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
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