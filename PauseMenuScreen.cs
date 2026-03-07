using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Data;
using ASTRANET_Hidden_Sector.Entities.Quest;
using ASTRANET_Hidden_Sector.Entities.Faction;
using ASTRANET_Hidden_Sector.World;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class PauseMenuScreen : GameScreen
    {
        private List<string> menuItems = new List<string> { "Продолжить", "Сохранить", "Загрузить", "Выход в главное меню" };
        private int selectedIndex = 0;

        public PauseMenuScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
        }

        public override void Render()
        {
            uiManager.Clear();

            int centerX = Console.WindowWidth / 2;
            int startY = Console.WindowHeight / 2 - menuItems.Count / 2;

            uiManager.SetCursorPosition(centerX - 10, startY - 2);
            uiManager.Write("=== ПАУЗА ===", ConsoleColor.Yellow);

            for (int i = 0; i < menuItems.Count; i++)
            {
                uiManager.SetCursorPosition(centerX - 10, startY + i);
                if (i == selectedIndex)
                    uiManager.Write("> ", ConsoleColor.Yellow);
                else
                    uiManager.Write("  ", ConsoleColor.Gray);
                uiManager.Write(menuItems[i], ConsoleColor.White);
            }

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("↑/↓ для выбора, Enter - подтвердить, Esc - вернуться в игру", ConsoleColor.DarkGray);
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + menuItems.Count) % menuItems.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % menuItems.Count;
                    break;
                case ConsoleKey.Enter:
                    ExecuteMenuItem();
                    break;
                case ConsoleKey.Escape:
                    stateManager.PopScreen();
                    break;
            }
        }

        private void ExecuteMenuItem()
        {
            switch (selectedIndex)
            {
                case 0:
                    stateManager.PopScreen();
                    break;
                case 1:
                    SaveGame();
                    break;
                case 2:
                    stateManager.PopScreen();
                    stateManager.PushScreen(new LoadGameMenuScreen(stateManager, uiManager));
                    break;
                case 3:
                    stateManager.ChangeScreen(new MainMenuScreen(stateManager, uiManager));
                    break;
            }
        }

        private void SaveGame()
        {
            var player = Game.CurrentPlayer;
            if (player == null) return;

            string currentScreenType = "Galaxy";
            string currentSectorName = "Альфа";
            string currentSystemName = "";
            int localX = 0, localY = 0;

            var galaxyScreen = stateManager.GetPreviousScreen<GalaxyMapScreen>();
            var globalScreen = stateManager.GetPreviousScreen<GlobalMapScreen>();
            var localScreen = stateManager.GetPreviousScreen<LocalMapScreen>();

            if (localScreen != null)
            {
                currentScreenType = "Local";
                var system = localScreen.ParentSystem;
                if (system != null)
                {
                    currentSectorName = Game.CurrentSector?.Name ?? "Альфа";
                    currentSystemName = system.Name;
                }
                localX = localScreen.GetPlayerX();
                localY = localScreen.GetPlayerY();
            }
            else if (globalScreen != null)
            {
                currentScreenType = "Sector";
                currentSectorName = globalScreen.CurrentSector.Name;
                currentSystemName = globalScreen.CurrentSystem?.Name ?? "";
            }
            else if (galaxyScreen != null)
            {
                currentScreenType = "Galaxy";
                currentSectorName = galaxyScreen.CurrentSector?.Name ?? "Альфа";
            }

            var playerData = new SaveData
            {
                PlayerName = player.Name,
                Level = player.Level,
                Experience = player.Experience,
                Health = player.Health,
                Credits = player.Credits,
                Strength = player.Strength,
                Dexterity = player.Dexterity,
                Intelligence = player.Intelligence,
                Luck = player.Luck,
                Charisma = player.Charisma,
                BackgroundId = player.Background?.Id ?? "",
                Inventory = player.Inventory.GetAllItems().ConvertAll(i => new SavedItem { Id = i.Id, Count = 1 }),
                Quests = QuestManager.ExportState(),
                Reputation = FactionManager.ExportReputation(),
                CurrentSector = currentSectorName,
                CurrentSystem = currentSystemName,
                PlayerX = localX,
                PlayerY = localY,
                InLocalMap = (currentScreenType == "Local")
            };

            SavedWorld worldData = null;
            if (Game.CurrentWorld != null)
            {
                worldData = new SavedWorld
                {
                    Seed = 12345,
                    Sectors = Game.CurrentWorld.Select(s => SaveSector(s)).ToList(),
                    CurrentScreen = currentScreenType,
                    CurrentSectorId = currentSectorName,
                    CurrentSystemId = currentSystemName,
                    LocalPlayerX = localX,
                    LocalPlayerY = localY
                };
            }

            SaveLoadManager.SaveGame(player.Name, playerData, worldData);
            stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Сохранение", "Игра сохранена."));
        }

        private SavedSector SaveSector(Sector sector)
        {
            return new SavedSector
            {
                Name = sector.Name,
                IsLocked = sector.IsLocked,
                X = sector.X,
                Y = sector.Y,
                ConnectedSectorIds = sector.ConnectedSectors.Select(s => s.Name).ToList(),
                Systems = sector.Systems.Select(s => SaveSystem(s)).ToList()
            };
        }

        private SavedStarSystem SaveSystem(StarSystem system)
        {
            return new SavedStarSystem
            {
                Name = system.Name,
                X = system.X,
                Y = system.Y,
                Type = system.Type,
                IsExplored = system.IsExplored,
                IsHidden = system.IsHidden,
                ConnectedSystemNames = system.ConnectedSystems.Select(s => s.Name).ToList(),
                LocalMap = system.LocalMap != null ? SaveLocalMap(system.LocalMap) : null
            };
        }

        private SavedLocalMap SaveLocalMap(LocalMap map)
        {
            var saved = new SavedLocalMap
            {
                Width = LocalMap.Width,
                Height = LocalMap.Height,
                Cells = new List<SavedMapCell>()
            };
            for (int x = 0; x < LocalMap.Width; x++)
            {
                for (int y = 0; y < LocalMap.Height; y++)
                {
                    var cell = map.GetCell(x, y);
                    if (cell != null)
                    {
                        saved.Cells.Add(new SavedMapCell
                        {
                            X = x,
                            Y = y,
                            IsVisible = cell.IsVisible,
                            IsExplored = cell.IsExplored,
                            Symbol = cell.Symbol,
                            Color = cell.Color,
                            Entity = cell.Entity != null ? SaveEntity(cell.Entity) : null
                        });
                    }
                }
            }
            return saved;
        }

        private SavedEntity SaveEntity(MapEntity entity)
        {
            var saved = new SavedEntity
            {
                Type = entity.GetType().Name,
                Name = entity.Name,
                Description = entity.Description,
                Symbol = entity.Symbol,
                Color = entity.Color,
                X = entity.X,
                Y = entity.Y,
                IsDestroyed = entity.IsDestroyed,
                IsStatic = entity.IsStatic
            };
            if (entity is StaticEntity staticEntity)
            {
                saved.LocationType = staticEntity.Type.ToString();
                saved.DialogueId = staticEntity.DialogueId;
            }
            return saved;
        }
    }
}