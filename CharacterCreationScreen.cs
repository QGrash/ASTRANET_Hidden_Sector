using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities;
using ASTRANET_Hidden_Sector.World;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class CharacterCreationScreen : GameScreen
    {
        private enum CreationStage { NameInput, BackgroundSelection, Confirmation }
        private CreationStage currentStage = CreationStage.NameInput;

        private string playerName = "";
        private List<Background> backgrounds;
        private int selectedBackgroundIndex = 0;
        private bool isEditingName = true;

        public CharacterCreationScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
            backgrounds = Background.GetAll();
        }

        public override void Render()
        {
            uiManager.Clear();

            int leftX = 5;
            int topY = 3;

            string header = "=== СОЗДАНИЕ ПЕРСОНАЖА ===";
            for (int i = 0; i < header.Length; i++)
                uiManager.SetPixel(leftX + i, topY - 1, header[i], ConsoleColor.Yellow);

            switch (currentStage)
            {
                case CreationStage.NameInput:
                    RenderNameInput(leftX, topY);
                    break;
                case CreationStage.BackgroundSelection:
                    RenderBackgroundSelection(leftX, topY);
                    break;
                case CreationStage.Confirmation:
                    RenderConfirmation(leftX, topY);
                    break;
            }

            string hint = "";
            switch (currentStage)
            {
                case CreationStage.NameInput:
                    hint = "Введите имя, Enter - подтвердить, Backspace - удалить символ";
                    break;
                case CreationStage.BackgroundSelection:
                    hint = "↑/↓ - выбор, Enter - подтвердить, Backspace - вернуться к имени";
                    break;
                case CreationStage.Confirmation:
                    hint = "Enter - начать игру, Backspace - изменить выбор";
                    break;
            }
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        private void RenderNameInput(int leftX, int topY)
        {
            string label = "Имя персонажа:";
            for (int i = 0; i < label.Length; i++)
                uiManager.SetPixel(leftX + i, topY, label[i], ConsoleColor.Cyan);

            string input = "> " + playerName + (isEditingName ? "_" : "");
            for (int i = 0; i < input.Length; i++)
                uiManager.SetPixel(leftX + i, topY + 2, input[i], ConsoleColor.White);
        }

        private void RenderBackgroundSelection(int leftX, int topY)
        {
            string title = "Выберите предысторию:";
            for (int i = 0; i < title.Length; i++)
                uiManager.SetPixel(leftX + i, topY, title[i], ConsoleColor.Cyan);

            int listStartY = topY + 2;
            for (int i = 0; i < backgrounds.Count; i++)
            {
                int y = listStartY + i;
                if (i == selectedBackgroundIndex)
                    uiManager.SetPixel(leftX, y, '>', ConsoleColor.Yellow);
                else
                    uiManager.SetPixel(leftX, y, ' ', ConsoleColor.Black);

                for (int j = 0; j < backgrounds[i].Name.Length; j++)
                    uiManager.SetPixel(leftX + 2 + j, y, backgrounds[i].Name[j],
                        i == selectedBackgroundIndex ? ConsoleColor.Yellow : ConsoleColor.Gray);
            }

            if (selectedBackgroundIndex >= 0 && selectedBackgroundIndex < backgrounds.Count)
            {
                var bg = backgrounds[selectedBackgroundIndex];
                int detailsX = leftX + 30;
                int detailsY = topY + 2;

                string desc = "Описание: " + bg.Description;
                for (int i = 0; i < desc.Length; i++)
                    uiManager.SetPixel(detailsX + i, detailsY, desc[i], ConsoleColor.DarkGray);
                detailsY++;

                string bonuses = "Бонусы к характеристикам:";
                for (int i = 0; i < bonuses.Length; i++)
                    uiManager.SetPixel(detailsX + i, detailsY, bonuses[i], ConsoleColor.Green);
                detailsY++;

                foreach (var bonus in bg.StatBonuses)
                {
                    string line = $"  {bonus.Key}: +{bonus.Value}";
                    for (int i = 0; i < line.Length; i++)
                        uiManager.SetPixel(detailsX + i, detailsY, line[i], ConsoleColor.White);
                    detailsY++;
                }

                string ability = "Способность: " + bg.AbilityName;
                for (int i = 0; i < ability.Length; i++)
                    uiManager.SetPixel(detailsX + i, detailsY, ability[i], ConsoleColor.Magenta);
                detailsY++;

                string effect = "  " + bg.AbilityEffect;
                for (int i = 0; i < effect.Length; i++)
                    uiManager.SetPixel(detailsX + i, detailsY, effect[i], ConsoleColor.DarkMagenta);
            }
        }

        private void RenderConfirmation(int leftX, int topY)
        {
            var bg = backgrounds[selectedBackgroundIndex];
            string confirm = "Подтвердите создание персонажа:";
            for (int i = 0; i < confirm.Length; i++)
                uiManager.SetPixel(leftX + i, topY, confirm[i], ConsoleColor.Yellow);

            string nameLine = $"Имя: {playerName}";
            for (int i = 0; i < nameLine.Length; i++)
                uiManager.SetPixel(leftX + i, topY + 2, nameLine[i], ConsoleColor.White);

            string bgLine = $"Предыстория: {bg.Name}";
            for (int i = 0; i < bgLine.Length; i++)
                uiManager.SetPixel(leftX + i, topY + 3, bgLine[i], ConsoleColor.White);

            string abilityLine = $"Способность: {bg.AbilityName}";
            for (int i = 0; i < abilityLine.Length; i++)
                uiManager.SetPixel(leftX + i, topY + 4, abilityLine[i], ConsoleColor.Magenta);
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (currentStage)
            {
                case CreationStage.NameInput:
                    HandleNameInput(key);
                    break;
                case CreationStage.BackgroundSelection:
                    HandleBackgroundSelection(key);
                    break;
                case CreationStage.Confirmation:
                    HandleConfirmation(key);
                    break;
            }
        }

        private void HandleNameInput(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Enter)
            {
                if (playerName.Length > 0)
                {
                    currentStage = CreationStage.BackgroundSelection;
                }
            }
            else if (key.Key == ConsoleKey.Backspace && playerName.Length > 0)
            {
                playerName = playerName.Substring(0, playerName.Length - 1);
            }
            else if (key.KeyChar >= 32 && key.KeyChar <= 126)
            {
                if (playerName.Length < 20)
                    playerName += key.KeyChar;
            }
        }

        private void HandleBackgroundSelection(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedBackgroundIndex = (selectedBackgroundIndex - 1 + backgrounds.Count) % backgrounds.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedBackgroundIndex = (selectedBackgroundIndex + 1) % backgrounds.Count;
                    break;
                case ConsoleKey.Enter:
                    currentStage = CreationStage.Confirmation;
                    break;
                case ConsoleKey.Backspace:
                    currentStage = CreationStage.NameInput;
                    break;
            }
        }

        private void HandleConfirmation(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Enter)
            {
                var player = new Player(playerName, backgrounds[selectedBackgroundIndex]);

                player.Inventory.AddItem(new Item("medkit", "Медпакет", "Восстанавливает 30 HP", ItemType.Consumable, 0.1f, 50) { HealAmount = 30 });
                player.Inventory.AddItem(new Item("medkit", "Медпакет", "Восстанавливает 30 HP", ItemType.Consumable, 0.1f, 50) { HealAmount = 30 });
                player.Inventory.AddItem(new Item("medkit", "Медпакет", "Восстанавливает 30 HP", ItemType.Consumable, 0.1f, 50) { HealAmount = 30 });
                player.Inventory.AddItem(new Item("stim", "Стимулятор", "Временно повышает характеристики", ItemType.Consumable, 0.1f, 40) { HealAmount = 10 });
                player.Inventory.AddItem(new Item("fuel", "Топливо", "Канистра с топливом", ItemType.Resource, 10f, 50));
                player.Inventory.AddItem(new Item("metal", "Металл", "Слиток металла", ItemType.Resource, 5f, 30));

                Game.CurrentPlayer = player;

                var generator = new WorldGenerator(12345);
                var sectors = generator.GenerateSectors();
                Game.CurrentWorld = sectors;
                var startSector = sectors.Where(s => !s.IsLocked).OrderBy(s => Guid.NewGuid()).FirstOrDefault() ?? sectors[0];

                stateManager.ChangeScreen(new GalaxyMapScreen(stateManager, uiManager, sectors, startSector));
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                currentStage = CreationStage.BackgroundSelection;
            }
        }
    }
}