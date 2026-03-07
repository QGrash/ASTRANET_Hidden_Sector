using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Data;
using ASTRANET_Hidden_Sector.Entities;
using ASTRANET_Hidden_Sector.Entities.Faction;
using ASTRANET_Hidden_Sector.Entities.Quest;
using ASTRANET_Hidden_Sector.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class LoadGameMenuScreen : GameScreen
    {
        private List<SaveMetadata> saves;
        private int selectedIndex = 0;

        public LoadGameMenuScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
            RefreshSaves();
        }

        private void RefreshSaves()
        {
            saves = SaveLoadManager.GetAllSaves();
            if (saves.Count == 0)
            {
                saves = new List<SaveMetadata> { new SaveMetadata { PlayerName = "Нет сохранений" } };
            }
        }

        public override void Render()
        {
            uiManager.Clear();

            int centerX = Console.WindowWidth / 2;
            int startY = Console.WindowHeight / 2 - saves.Count * 3 / 2;

            uiManager.SetCursorPosition(centerX - 10, startY - 3);
            uiManager.Write("=== ЗАГРУЗКА ИГРЫ ===", ConsoleColor.Yellow);

            for (int i = 0; i < saves.Count; i++)
            {
                var save = saves[i];
                int lineY = startY + i * 3;

                uiManager.SetCursorPosition(centerX - 30, lineY);
                if (i == selectedIndex)
                    uiManager.Write("> ", ConsoleColor.Yellow);
                else
                    uiManager.Write("  ", ConsoleColor.Gray);

                if (save.PlayerName == "Нет сохранений")
                {
                    uiManager.Write(save.PlayerName, ConsoleColor.DarkGray);
                }
                else
                {
                    uiManager.Write($"{save.PlayerName} (ур. {save.Level})", ConsoleColor.White);
                    uiManager.SetCursorPosition(centerX - 20, lineY + 1);
                    uiManager.Write($"Сектор: {save.Sector} | Система: {save.System}", ConsoleColor.Cyan);
                    uiManager.SetCursorPosition(centerX - 20, lineY + 2);
                    uiManager.Write($"Сохранено: {save.SaveTime:dd.MM.yyyy HH:mm}", ConsoleColor.DarkGray);
                }
            }

            uiManager.SetCursorPosition(2, Console.WindowHeight - 3);
            if (saves.Count > 0 && saves[0].PlayerName != "Нет сохранений")
            {
                uiManager.Write("↑/↓ - выбор, Enter - загрузить, Del - удалить, Esc - назад", ConsoleColor.DarkGray);
            }
            else
            {
                uiManager.Write("Esc - назад", ConsoleColor.DarkGray);
            }
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            if (saves.Count == 0 || (saves.Count == 1 && saves[0].PlayerName == "Нет сохранений"))
            {
                if (key.Key == ConsoleKey.Escape)
                    stateManager.PopScreen();
                return;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + saves.Count) % saves.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % saves.Count;
                    break;
                case ConsoleKey.Enter:
                    LoadSelectedGame();
                    break;
                case ConsoleKey.Delete:
                    DeleteSelectedSave();
                    break;
                case ConsoleKey.Escape:
                    stateManager.PopScreen();
                    break;
            }
        }

        private void LoadSelectedGame()
        {
            if (saves.Count == 0 || saves[0].PlayerName == "Нет сохранений") return;

            string fileName = saves[selectedIndex].FileName;
            var combined = SaveLoadManager.LoadGame(fileName);
            if (combined == null)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Ошибка",
                    "Не удалось загрузить сохранение."));
                return;
            }

            var playerData = combined.Player;
            if (playerData == null)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Ошибка",
                    "Данные игрока повреждены."));
                return;
            }

            var background = Background.GetById(playerData.BackgroundId);
            if (background == null)
            {
                background = Background.GetAll().FirstOrDefault();
                if (background == null)
                {
                    stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Ошибка",
                        "Не удалось загрузить предысторию персонажа."));
                    return;
                }
            }

            var player = new Player(playerData.PlayerName, background);
            player.Level = playerData.Level;
            player.Experience = playerData.Experience;
            player.Health = playerData.Health;
            player.Credits = playerData.Credits;
            player.Strength = playerData.Strength;
            player.Dexterity = playerData.Dexterity;
            player.Intelligence = playerData.Intelligence;
            player.Luck = playerData.Luck;
            player.Charisma = playerData.Charisma;

            foreach (var savedItem in playerData.Inventory)
            {
                var item = new Item(savedItem.Id, savedItem.Id, "Восстановленный предмет", ItemType.Misc, 0, 0);
                player.Inventory.AddItem(item);
            }

            Game.CurrentPlayer = player;
            QuestManager.ImportState(playerData.Quests);
            FactionManager.ImportReputation(playerData.Reputation);

            var worldData = combined.World;
            if (worldData != null && worldData.Sectors.Count > 0)
            {
                var sectors = new List<Sector>();
                var sectorDict = new Dictionary<string, Sector>();

                foreach (var savedSector in worldData.Sectors)
                {
                    var sector = new Sector(savedSector, sectorDict);
                    sectorDict[sector.Name] = sector;
                    sectors.Add(sector);
                }

                for (int i = 0; i < worldData.Sectors.Count; i++)
                {
                    var savedSector = worldData.Sectors[i];
                    var sector = sectors[i];
                    foreach (var connName in savedSector.ConnectedSectorIds)
                    {
                        if (sectorDict.TryGetValue(connName, out var connSector))
                        {
                            sector.ConnectedSectors.Add(connSector);
                        }
                    }
                }

                for (int i = 0; i < worldData.Sectors.Count; i++)
                {
                    var savedSector = worldData.Sectors[i];
                    var sector = sectors[i];
                    var systems = new List<StarSystem>();
                    var systemDict = new Dictionary<string, StarSystem>();

                    foreach (var savedSystem in savedSector.Systems)
                    {
                        var system = new StarSystem(savedSystem, new List<StarSystem>());
                        systemDict[system.Name] = system;
                        systems.Add(system);
                    }

                    for (int j = 0; j < savedSector.Systems.Count; j++)
                    {
                        var savedSystem = savedSector.Systems[j];
                        var system = systems[j];
                        foreach (var connName in savedSystem.ConnectedSystemNames)
                        {
                            if (systemDict.TryGetValue(connName, out var connSystem))
                            {
                                system.ConnectedSystems.Add(connSystem);
                            }
                        }
                    }

                    sector.Systems = systems;
                }

                Game.CurrentWorld = sectors;

                var targetSector = sectors.FirstOrDefault(s => s.Name == worldData.CurrentSectorId);
                if (targetSector == null) targetSector = sectors[0];
                var targetSystem = targetSector.Systems.FirstOrDefault(s => s.Name == worldData.CurrentSystemId);
                if (targetSystem == null && targetSector.Systems.Count > 0)
                    targetSystem = targetSector.Systems[0];

                Game.CurrentSector = targetSector;
                Game.CurrentSystem = targetSystem;

                if (worldData.CurrentScreen == "Local" && targetSystem != null)
                {
                    if (targetSystem.LocalMap != null)
                    {
                        targetSystem.LocalMap.PlayerX = worldData.LocalPlayerX;
                        targetSystem.LocalMap.PlayerY = worldData.LocalPlayerY;
                    }
                    stateManager.ChangeScreen(new LocalMapScreen(stateManager, uiManager, targetSystem));
                }
                else if (worldData.CurrentScreen == "Sector" && targetSystem != null)
                {
                    stateManager.ChangeScreen(new GlobalMapScreen(stateManager, uiManager, targetSector, targetSystem));
                }
                else
                {
                    stateManager.ChangeScreen(new GalaxyMapScreen(stateManager, uiManager, sectors, targetSector));
                }
            }
            else
            {
                var generator = new WorldGenerator(12345);
                var sectors = generator.GenerateSectors();
                var startSector = sectors.First(s => !s.IsLocked);
                Game.CurrentWorld = sectors;
                Game.CurrentSector = startSector;
                Game.CurrentSystem = null;
                stateManager.ChangeScreen(new GalaxyMapScreen(stateManager, uiManager, sectors, startSector));
            }
        }

        private void DeleteSelectedSave()
        {
            if (saves.Count == 0 || saves[0].PlayerName == "Нет сохранений") return;

            string fileName = saves[selectedIndex].FileName;
            string message = $"Удалить сохранение \"{fileName}\"?";
            stateManager.PushScreen(new ConfirmScreen(stateManager, uiManager, message, (confirmed) =>
            {
                if (confirmed)
                {
                    SaveLoadManager.DeleteSave(fileName);
                    RefreshSaves();
                    selectedIndex = 0;
                }
            }));
        }
    }
}