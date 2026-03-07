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

        // Для ввода имени
        private bool isEditingName = true;

        public CharacterCreationScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
            backgrounds = Background.GetAll();
        }

        public override void Render()
        {
            uiManager.Clear();

            int centerX = Console.WindowWidth / 2;
            int startY = 5;

            // Заголовок
            uiManager.SetCursorPosition(centerX - 15, startY - 2);
            uiManager.Write("=== СОЗДАНИЕ ПЕРСОНАЖА ===", ConsoleColor.Yellow);

            switch (currentStage)
            {
                case CreationStage.NameInput:
                    RenderNameInput(centerX, startY);
                    break;
                case CreationStage.BackgroundSelection:
                    RenderBackgroundSelection(centerX, startY);
                    break;
                case CreationStage.Confirmation:
                    RenderConfirmation(centerX, startY);
                    break;
            }

            // Подсказки внизу
            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            switch (currentStage)
            {
                case CreationStage.NameInput:
                    uiManager.Write("Введите имя, Enter - подтвердить, Backspace - удалить символ", ConsoleColor.DarkGray);
                    break;
                case CreationStage.BackgroundSelection:
                    uiManager.Write("↑/↓ - выбор, Enter - подтвердить, Backspace - вернуться к имени", ConsoleColor.DarkGray);
                    break;
                case CreationStage.Confirmation:
                    uiManager.Write("Enter - начать игру, Backspace - изменить выбор", ConsoleColor.DarkGray);
                    break;
            }
        }

        private void RenderNameInput(int centerX, int startY)
        {
            uiManager.SetCursorPosition(centerX - 10, startY);
            uiManager.Write("Имя персонажа:", ConsoleColor.Cyan);
            uiManager.SetCursorPosition(centerX - 10, startY + 2);
            uiManager.Write("> " + playerName + (isEditingName ? "_" : ""), ConsoleColor.White);
        }

        private void RenderBackgroundSelection(int centerX, int startY)
        {
            uiManager.SetCursorPosition(centerX - 15, startY);
            uiManager.Write("Выберите предысторию:", ConsoleColor.Cyan);

            int listStartY = startY + 2;
            for (int i = 0; i < backgrounds.Count; i++)
            {
                uiManager.SetCursorPosition(centerX - 20, listStartY + i);
                if (i == selectedBackgroundIndex)
                    uiManager.Write("> " + backgrounds[i].Name, ConsoleColor.Yellow);
                else
                    uiManager.Write("  " + backgrounds[i].Name, ConsoleColor.Gray);
            }

            // Отображение деталей выбранной предыстории
            if (selectedBackgroundIndex >= 0 && selectedBackgroundIndex < backgrounds.Count)
            {
                var bg = backgrounds[selectedBackgroundIndex];
                int detailsX = centerX + 10;
                int detailsY = startY + 2;

                uiManager.SetCursorPosition(detailsX, detailsY++);
                uiManager.Write("Описание: " + bg.Description, ConsoleColor.DarkGray);
                uiManager.SetCursorPosition(detailsX, detailsY++);
                uiManager.Write("Бонусы к характеристикам:", ConsoleColor.Green);
                foreach (var bonus in bg.StatBonuses)
                {
                    uiManager.SetCursorPosition(detailsX, detailsY++);
                    uiManager.Write($"  {bonus.Key}: +{bonus.Value}", ConsoleColor.White);
                }
                uiManager.SetCursorPosition(detailsX, detailsY++);
                uiManager.Write("Способность: " + bg.AbilityName, ConsoleColor.Magenta);
                uiManager.SetCursorPosition(detailsX, detailsY++);
                uiManager.Write("  " + bg.AbilityEffect, ConsoleColor.DarkMagenta);
            }
        }

        private void RenderConfirmation(int centerX, int startY)
        {
            var bg = backgrounds[selectedBackgroundIndex];
            uiManager.SetCursorPosition(centerX - 15, startY);
            uiManager.Write("Подтвердите создание персонажа:", ConsoleColor.Yellow);

            uiManager.SetCursorPosition(centerX - 10, startY + 2);
            uiManager.Write($"Имя: {playerName}", ConsoleColor.White);
            uiManager.SetCursorPosition(centerX - 10, startY + 3);
            uiManager.Write($"Предыстория: {bg.Name}", ConsoleColor.White);
            uiManager.SetCursorPosition(centerX - 10, startY + 4);
            uiManager.Write($"Способность: {bg.AbilityName}", ConsoleColor.Magenta);
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
                Game.CurrentWorld = sectors; // <--- ВАЖНО
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