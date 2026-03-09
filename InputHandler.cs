using ASTRANET_Hidden_Sector.Data;
using ASTRANET_Hidden_Sector.Entities.Dialogue;
using ASTRANET_Hidden_Sector.Screens;
using ASTRANET_Hidden_Sector.Entities.Quest;
using ASTRANET_Hidden_Sector.Entities.Faction;
using System;
using ASTRANET_Hidden_Sector.World;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Entities.Trade;

namespace ASTRANET_Hidden_Sector.Core
{
    public class InputHandler
    {
        private GameStateManager stateManager;

        public InputHandler(GameStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public void ProcessInput()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (!HandleGlobalKeys(key))
                {
                    stateManager.HandleInput(key);
                }
            }
        }

        private bool HandleGlobalKeys(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.F5:
                    if (Game.CurrentPlayer != null)
                        SaveGame();
                    else
                        stateManager.PushScreen(new MessageScreen(stateManager, stateManager.UIManager,
                            "Сохранение", "Нет активной игры."));
                    return true;

                case ConsoleKey.Escape:
                    if (Game.CurrentPlayer != null)
                    {
                        stateManager.PushScreen(new PauseMenuScreen(stateManager, stateManager.UIManager));
                        return true;
                    }
                    break;
            }

            if (Game.CurrentPlayer == null)
                return false;

            switch (key.Key)
            {
                case ConsoleKey.I:
                    stateManager.PushScreen(new InventoryScreen(stateManager, stateManager.UIManager));
                    return true;
                case ConsoleKey.C:
                    stateManager.PushScreen(new CharacterScreen(stateManager, stateManager.UIManager));
                    return true;
                case ConsoleKey.B:
                    stateManager.PushScreen(new ShipScreen(stateManager, stateManager.UIManager));
                    return true;
                case ConsoleKey.J:
                    stateManager.PushScreen(new QuestLogScreen(stateManager, stateManager.UIManager));
                    return true;
                case ConsoleKey.R:
                    stateManager.PushScreen(new ReputationScreen(stateManager, stateManager.UIManager));
                    return true;
            }

            return false;
        }

        private void SaveGame()
        {
            var player = Game.CurrentPlayer;
            if (player == null) return;

            string currentScreenType = "Galaxy";
            string currentSectorName = "Альфа";
            string currentSystemName = "";
            int localX = 0, localY = 0;

            var galaxyScreen = stateManager.GetCurrentScreen<GalaxyMapScreen>();
            var globalScreen = stateManager.GetCurrentScreen<GlobalMapScreen>();
            var localScreen = stateManager.GetCurrentScreen<LocalMapScreen>();
            var interiorScreen = stateManager.GetCurrentScreen<InteriorMapScreen>();

            if (interiorScreen != null)
            {
                currentScreenType = "Interior";
                currentSectorName = Game.CurrentSector?.Name ?? "Альфа";
                currentSystemName = Game.CurrentSystem?.Name ?? "";
                localX = interiorScreen.GetPlayerX();
                localY = interiorScreen.GetPlayerY();
            }
            else if (localScreen != null)
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
            stateManager.PushScreen(new MessageScreen(stateManager, stateManager.UIManager, "Сохранение", "Игра сохранена."));
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