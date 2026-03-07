using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.World;
using ASTRANET_Hidden_Sector.Entities;
using ASTRANET_Hidden_Sector.Entities.Dialogue;
using ASTRANET_Hidden_Sector.Entities.Quest;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class LocalMapScreen : GameScreen
    {
        private LocalMap map;
        private StarSystem parentSystem;
        private const int MAP_OFFSET_X = 2;
        private const int MAP_OFFSET_Y = 3;
        private const int CELL_WIDTH = 2;

        // Для мерцания фона
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
            try
            {
                uiManager.Clear();

                uiManager.SetCursorPosition(2, 1);
                uiManager.Write($"Локальная карта: {parentSystem.Name} ({parentSystem.Type})", ConsoleColor.Cyan);

                DrawFrame();

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
                                color = GetTwinkleColor(x, y, explored: false);
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
                                color = GetTwinkleColor(x, y, explored: true);
                            }
                        }
                        else
                        {
                            symbol = '·';
                            color = ConsoleColor.DarkBlue;
                        }

                        uiManager.SetCursorPosition(screenX, screenY);
                        uiManager.Write(symbol.ToString(), color);
                        uiManager.Write(" ", ConsoleColor.Black);
                    }
                }

                var currentCell = map.GetCell(map.PlayerX, map.PlayerY)!;
                if (currentCell.Entity != null && !currentCell.Entity.IsDestroyed)
                {
                    var entity = currentCell.Entity;
                    int infoX = 50;
                    int infoY = 3;
                    uiManager.SetCursorPosition(infoX, infoY++);
                    uiManager.Write($"Объект: {entity.Name}", ConsoleColor.Yellow);
                    uiManager.SetCursorPosition(infoX, infoY++);
                    uiManager.Write($"Тип: {entity.GetType().Name}", ConsoleColor.Green);
                    uiManager.SetCursorPosition(infoX, infoY++);
                    uiManager.Write(entity.Description, ConsoleColor.Gray);
                }

                uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
                uiManager.Write("Стрелки - перемещение, E - взаимодействие, I - инвентарь, C - персонаж, B - корабль, J - квесты, Backspace - назад, ESC - меню", ConsoleColor.DarkGray);
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Ошибка в LocalMapScreen.Render: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }
        }

        private void DrawFrame()
        {
            int left = MAP_OFFSET_X - 1;
            int right = left + LocalMap.Width * CELL_WIDTH;
            int top = MAP_OFFSET_Y - 1;
            int bottom = top + LocalMap.Height + 1;

            for (int x = left + 1; x < right; x++)
            {
                uiManager.SetCursorPosition(x, top);
                uiManager.Write("─", ConsoleColor.Gray);
                uiManager.SetCursorPosition(x, bottom);
                uiManager.Write("─", ConsoleColor.Gray);
            }

            for (int y = top + 1; y < bottom; y++)
            {
                uiManager.SetCursorPosition(left, y);
                uiManager.Write("│", ConsoleColor.Gray);
                uiManager.SetCursorPosition(right, y);
                uiManager.Write("│", ConsoleColor.Gray);
            }

            uiManager.SetCursorPosition(left, top);
            uiManager.Write("┌", ConsoleColor.Gray);
            uiManager.SetCursorPosition(right, top);
            uiManager.Write("┐", ConsoleColor.Gray);
            uiManager.SetCursorPosition(left, bottom);
            uiManager.Write("└", ConsoleColor.Gray);
            uiManager.SetCursorPosition(right, bottom);
            uiManager.Write("┘", ConsoleColor.Gray);
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

            try
            {
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
                        Console.WriteLine("LocalMapScreen: Backspace pressed. Popping screen...");
                        stateManager.PopScreen();
                        Console.WriteLine("LocalMapScreen: PopScreen completed.");
                        return;
                    default:
                        return;
                }

                if (newX >= 0 && newX < LocalMap.Width && newY >= 0 && newY < LocalMap.Height)
                {
                    map.PlayerX = newX;
                    map.PlayerY = newY;
                    map.UpdateVisibility();
                    map.UpdateAllEntities();
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Ошибка в LocalMapScreen.HandleInput: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }
        }

        private void InteractWithCurrentCell()
        {
            try
            {
                var cell = map.GetCell(map.PlayerX, map.PlayerY)!;
                if (cell.IsExplored && cell.Entity != null && !cell.Entity.IsDestroyed)
                {
                    QuestManager.UpdateProgress(ObjectiveType.VisitLocation, cell.Entity.Name, 1);
                    cell.Entity.Interact(stateManager, uiManager);

                    if (cell.Entity.IsDestroyed)
                    {
                        cell.Entity = null;
                        map.UpdateCellAppearance(map.PlayerX, map.PlayerY);
                    }
                }
                else
                {
                    stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Пусто", "Здесь ничего нет."));
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Ошибка в LocalMapScreen.InteractWithCurrentCell: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }
        }

        public int GetPlayerX() => map.PlayerX;
        public int GetPlayerY() => map.PlayerY;
        public StarSystem ParentSystem => parentSystem;
    }
}