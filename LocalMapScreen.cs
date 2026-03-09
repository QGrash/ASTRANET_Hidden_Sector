using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.World;
using ASTRANET_Hidden_Sector.Entities;
using ASTRANET_Hidden_Sector.Entities.Dialogue;
using ASTRANET_Hidden_Sector.Entities.Quest;
using ASTRANET_Hidden_Sector.Combat;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class LocalMapScreen : GameScreen
    {
        private LocalMap map;
        private StarSystem parentSystem;
        private const int MAP_OFFSET_X = 2;
        private const int MAP_OFFSET_Y = 3;
        private const int CELL_WIDTH = 2;

        private float globalTime = 0f;
        private const float TWINKLE_SPEED = 2f;

        public LocalMapScreen(GameStateManager stateManager, UIManager uiManager, StarSystem system)
            : base(stateManager, uiManager)
        {
            parentSystem = system;
            if (system.LocalMap == null)
            {
                system.LocalMap = LocalMapGenerator.Generate(system);
            }
            map = system.LocalMap;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            globalTime += deltaTime * TWINKLE_SPEED;
        }

        public override void Render()
        {
            uiManager.Clear();

            // Заголовок
            string title = $"Локальная карта: {parentSystem.Name} ({parentSystem.Type})";
            for (int i = 0; i < title.Length; i++)
                uiManager.SetPixel(MAP_OFFSET_X + i, 1, title[i], ConsoleColor.Cyan);

            // Отрисовка клеток карты (без рамки)
            for (int y = 0; y < LocalMap.Height; y++)
            {
                for (int x = 0; x < LocalMap.Width; x++)
                {
                    var cell = map.GetCell(x, y)!;
                    int screenX = MAP_OFFSET_X + x * CELL_WIDTH;
                    int screenY = MAP_OFFSET_Y + y;

                    char symbol;
                    ConsoleColor color;

                    if (x == map.PlayerX && y == map.PlayerY)
                    {
                        symbol = 'Y';
                        color = ConsoleColor.White;
                    }
                    else if (cell.IsVisible)
                    {
                        if (cell.Entity != null && !cell.Entity.IsDestroyed)
                        {
                            symbol = cell.Symbol;
                            color = cell.Color;
                        }
                        else
                        {
                            symbol = '·';
                            color = GetTwinkleColor(x, y, false);
                        }
                    }
                    else if (cell.IsExplored)
                    {
                        if (cell.Entity != null && !cell.Entity.IsDestroyed && cell.Entity.IsStatic)
                        {
                            symbol = cell.Symbol;
                            color = ConsoleColor.Cyan;
                        }
                        else
                        {
                            symbol = '·';
                            color = GetTwinkleColor(x, y, true);
                        }
                    }
                    else
                    {
                        symbol = '·';
                        color = ConsoleColor.DarkBlue;
                    }

                    uiManager.SetPixel(screenX, screenY, symbol, color);
                    uiManager.SetPixel(screenX + 1, screenY, ' ', ConsoleColor.Black);
                }
            }

            // Информация о текущей клетке
            var currentCell = map.GetCell(map.PlayerX, map.PlayerY)!;
            if (currentCell.Entity != null && !currentCell.Entity.IsDestroyed)
            {
                var entity = currentCell.Entity;
                int infoX = 50;
                int infoY = 3;

                string label1 = $"Объект: {entity.Name}";
                for (int i = 0; i < label1.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, label1[i], ConsoleColor.Yellow);
                infoY++;

                string label2 = $"Тип: {entity.GetType().Name}";
                for (int i = 0; i < label2.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, label2[i], ConsoleColor.Green);
                infoY++;

                for (int i = 0; i < entity.Description.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, entity.Description[i], ConsoleColor.Gray);
            }

            // Подсказка
            string hint = "Стрелки - перемещение, E - взаимодействие, I - инвентарь, C - персонаж, B - корабль, J - квесты, Backspace - назад, ESC - меню";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        private ConsoleColor GetTwinkleColor(int x, int y, bool explored = false)
        {
            float brightness = (float)Math.Sin(globalTime + x * 0.2 + y * 0.3) * 0.5f + 0.5f;

            if (explored)
            {
                if (brightness > 0.7f) return ConsoleColor.White;
                if (brightness > 0.4f) return ConsoleColor.Cyan;
                return ConsoleColor.DarkCyan;
            }
            else
            {
                if (brightness > 0.8f) return ConsoleColor.White;
                if (brightness > 0.5f) return ConsoleColor.Gray;
                if (brightness > 0.2f) return ConsoleColor.DarkGray;
                return ConsoleColor.Black;
            }
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            int newX = map.PlayerX;
            int newY = map.PlayerY;

            switch (key.Key)
            {
                case ConsoleKey.UpArrow: newY--; break;
                case ConsoleKey.DownArrow: newY++; break;
                case ConsoleKey.LeftArrow: newX--; break;
                case ConsoleKey.RightArrow: newX++; break;
                case ConsoleKey.E:
                    InteractWithCurrentCell();
                    return;
                case ConsoleKey.Backspace:
                    stateManager.PopScreen();
                    return;
                default:
                    return;
            }

            if (newX >= 0 && newX < LocalMap.Width && newY >= 0 && newY < LocalMap.Height)
            {
                var targetCell = map.GetCell(newX, newY);
                if (targetCell != null)
                {
                    if (targetCell.Entity != null && targetCell.Entity.IsSolid)
                        return;
                    map.PlayerX = newX;
                    map.PlayerY = newY;
                    map.UpdateVisibility();
                    map.UpdateAllEntities();
                }
            }
        }

        private void InteractWithCurrentCell()
        {
            var cell = map.GetCell(map.PlayerX, map.PlayerY)!;
            if (cell.IsExplored && cell.Entity != null && !cell.Entity.IsDestroyed)
            {
                if (cell.Entity is StaticEntity staticEntity && staticEntity.Type == LocationType.Enemy)
                {
                    StartSpaceCombat(staticEntity);
                }
                else
                {
                    QuestManager.UpdateProgress(ObjectiveType.VisitLocation, cell.Entity.Name, 1);
                    cell.Entity.Interact(stateManager, uiManager);
                }
            }
            else
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Пусто", "Здесь ничего нет."));
            }
        }

        private void StartSpaceCombat(StaticEntity enemyEntity)
        {
            var playerShip = new SpaceCombatant(
                name: Game.CurrentPlayer?.Name ?? "Игрок",
                maxHealth: 100,
                maxEnergy: 50,
                shield: new Shield(50, 5),
                evasion: 10,
                weapons: new List<Weapon>
                {
                    new Weapon("Лазер", DamageType.Laser, 10, 15, 5, 10, 1, 5, 1.5f, WeaponClass.Medium, WeaponMode.Single)
                }
            );

            var enemyShip = new SpaceCombatant(
                name: enemyEntity.Name,
                maxHealth: 80,
                maxEnergy: 40,
                shield: new Shield(30, 2),
                evasion: 5,
                weapons: new List<Weapon>
                {
                    new Weapon("Плазма", DamageType.Plasma, 8, 12, 0, 8, 1, 10, 2.0f, WeaponClass.Medium, WeaponMode.Single)
                }
            );

            stateManager.PushScreen(new SpaceCombatScreen(stateManager, uiManager, playerShip, new List<SpaceCombatant> { enemyShip }, parentSystem));
        }

        public int GetPlayerX() => map.PlayerX;
        public int GetPlayerY() => map.PlayerY;
        public StarSystem ParentSystem => parentSystem;
    }
}