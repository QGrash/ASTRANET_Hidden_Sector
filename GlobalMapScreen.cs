using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.World;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class GlobalMapScreen : GameScreen
    {
        private Sector currentSector;
        private StarSystem currentSystem;
        private int selectedSystemIndex;

        private struct Star
        {
            public int X;
            public int Y;
            public char Symbol;
            public ConsoleColor Color;
        }
        private Star[] stars;
        private Random rand = new Random();
        private int starOffset = 0;

        private const int MAP_LEFT = 5;
        private const int MAP_TOP = 5;
        private const int MAP_WIDTH = 50;
        private const int MAP_HEIGHT = 20;

        public Sector CurrentSector => currentSector;
        public StarSystem CurrentSystem => currentSystem;

        public GlobalMapScreen(GameStateManager stateManager, UIManager uiManager, Sector sector, StarSystem startSystem)
            : base(stateManager, uiManager)
        {
            currentSector = sector;
            currentSystem = startSystem;
            selectedSystemIndex = currentSector.Systems.IndexOf(startSystem);
            if (selectedSystemIndex == -1) selectedSystemIndex = 0;

            stars = new Star[100];
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].X = rand.Next(0, Console.WindowWidth);
                stars[i].Y = rand.Next(0, Console.WindowHeight);
                stars[i].Symbol = rand.Next(2) == 0 ? '.' : '·';
                stars[i].Color = rand.Next(3) == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray;
            }
        }

        public override void Render()
        {
            uiManager.Clear();

            DrawStars();

            var systems = currentSector.Systems;
            DrawSectorFrame();

            var screenPositions = new List<(int x, int y, StarSystem sys)>();
            foreach (var sys in systems)
            {
                int screenX = MAP_LEFT + (int)((sys.X / 100.0) * MAP_WIDTH);
                int screenY = MAP_TOP + (int)((sys.Y / 100.0) * MAP_HEIGHT);
                screenPositions.Add((screenX, screenY, sys));
            }

            DrawConnections(screenPositions);

            for (int i = 0; i < screenPositions.Count; i++)
            {
                var (x, y, sys) = screenPositions[i];
                bool isSelected = (i == selectedSystemIndex);

                char symbol = GetSystemSymbol(sys.Type);
                ConsoleColor color = GetSystemColor(sys.Type);

                if (sys.IsHidden && !sys.IsExplored)
                {
                    symbol = '?';
                    color = ConsoleColor.DarkGray;
                }

                if (isSelected)
                {
                    uiManager.SetPixel(x - 1, y, '[', ConsoleColor.Yellow);
                    uiManager.SetPixel(x + 1, y, ']', ConsoleColor.Yellow);
                }

                uiManager.SetPixel(x, y, symbol, color);
            }

            if (currentSystem != null)
            {
                int curX = MAP_LEFT + (int)((currentSystem.X / 100.0) * MAP_WIDTH);
                int curY = MAP_TOP + (int)((currentSystem.Y / 100.0) * MAP_HEIGHT);
                uiManager.SetPixel(curX - 1, curY - 1, '[', ConsoleColor.Green);
                uiManager.SetPixel(curX + 1, curY, ']', ConsoleColor.Green);
            }

            DrawInfoPanel(screenPositions);

            string hint = "↑/↓/←/→ - выбор системы, Enter - переход (если есть связь), G - карта галактики, I - инвентарь, C - персонаж, B - корабль, J - квесты, Backspace - назад, ESC - меню";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        private void DrawStars()
        {
            starOffset = (starOffset + 1) % Console.WindowHeight;
            for (int i = 0; i < stars.Length; i++)
            {
                int y = (stars[i].Y + starOffset) % Console.WindowHeight;
                uiManager.SetPixel(stars[i].X, y, stars[i].Symbol, stars[i].Color);
            }
        }

        private void DrawSectorFrame()
        {
            int left = MAP_LEFT - 1;
            int right = left + MAP_WIDTH + 1;
            int top = MAP_TOP - 1;
            int bottom = top + MAP_HEIGHT + 1;

            for (int x = left + 1; x < right; x++)
            {
                uiManager.SetPixel(x, top, '─', ConsoleColor.DarkGray);
                uiManager.SetPixel(x, bottom, '─', ConsoleColor.DarkGray);
            }
            for (int y = top + 1; y < bottom; y++)
            {
                uiManager.SetPixel(left, y, '│', ConsoleColor.DarkGray);
                uiManager.SetPixel(right, y, '│', ConsoleColor.DarkGray);
            }
            uiManager.SetPixel(left, top, '┌', ConsoleColor.DarkGray);
            uiManager.SetPixel(right, top, '┐', ConsoleColor.DarkGray);
            uiManager.SetPixel(left, bottom, '└', ConsoleColor.DarkGray);
            uiManager.SetPixel(right, bottom, '┘', ConsoleColor.DarkGray);

            string name = $"Сектор: {currentSector.Name}";
            for (int i = 0; i < name.Length; i++)
                uiManager.SetPixel(MAP_LEFT + i, MAP_TOP - 2, name[i], ConsoleColor.Cyan);
        }

        private void DrawConnections(List<(int x, int y, StarSystem sys)> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                var (x1, y1, sys1) = positions[i];
                foreach (var conn in sys1.ConnectedSystems)
                {
                    var connPos = positions.Find(p => p.sys == conn);
                    if (connPos.sys != null)
                    {
                        DrawLine(x1, y1, connPos.x, connPos.y);
                    }
                }
            }
        }

        private void DrawLine(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (!IsSystemAt(x1, y1))
                {
                    uiManager.SetPixel(x1, y1, '·', ConsoleColor.DarkGray);
                }

                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }

        private bool IsSystemAt(int x, int y) => false;

        private char GetSystemSymbol(SystemType type)
        {
            return type switch
            {
                SystemType.Habitable => 'H',
                SystemType.Desert => 'D',
                SystemType.Hostile => 'X',
                SystemType.Anomalous => '?',
                SystemType.Hidden => '·',
                _ => '*'
            };
        }

        private ConsoleColor GetSystemColor(SystemType type)
        {
            return type switch
            {
                SystemType.Habitable => ConsoleColor.Green,
                SystemType.Desert => ConsoleColor.Yellow,
                SystemType.Hostile => ConsoleColor.Red,
                SystemType.Anomalous => ConsoleColor.Magenta,
                SystemType.Hidden => ConsoleColor.DarkGray,
                _ => ConsoleColor.White
            };
        }

        private void DrawInfoPanel(List<(int x, int y, StarSystem sys)> positions)
        {
            int infoX = MAP_LEFT + MAP_WIDTH + 5;
            int infoY = MAP_TOP;

            string line = "=== ИНФОРМАЦИЯ ===";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Yellow);
            infoY++;

            line = $"Текущий сектор: {currentSector.Name}";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.White);
            infoY++;

            line = $"Текущая система: {currentSystem?.Name ?? "нет"}";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Green);
            infoY++;

            if (selectedSystemIndex >= 0 && selectedSystemIndex < positions.Count)
            {
                var sys = positions[selectedSystemIndex].sys;
                infoY++;
                line = "Выбранная система:";
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Yellow);
                infoY++;

                line = $"Название: {sys.Name}";
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.White);
                infoY++;

                line = $"Тип: {sys.Type}";
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, line[i], GetSystemColor(sys.Type));
                infoY++;

                line = $"Координаты: {sys.X}, {sys.Y}";
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.DarkGray);
                infoY++;

                line = "Связи:";
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Cyan);
                infoY++;

                foreach (var conn in sys.ConnectedSystems)
                {
                    line = $"  {conn.Name}";
                    for (int i = 0; i < line.Length; i++)
                        uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Gray);
                    infoY++;
                }

                line = $"Соединена с текущей: {(currentSystem != null && currentSystem.ConnectedSystems.Contains(sys) ? "ДА" : "НЕТ")}";
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, line[i],
                        (currentSystem != null && currentSystem.ConnectedSystems.Contains(sys)) ? ConsoleColor.Green : ConsoleColor.Red);
            }
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            var systems = currentSector.Systems;

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    MoveSelectionVertical(systems, -1);
                    break;
                case ConsoleKey.DownArrow:
                    MoveSelectionVertical(systems, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    MoveSelectionHorizontal(systems, -1);
                    break;
                case ConsoleKey.RightArrow:
                    MoveSelectionHorizontal(systems, 1);
                    break;
                case ConsoleKey.G:
                    stateManager.PopScreen();
                    break;
                case ConsoleKey.Enter:
                    TryEnterSelectedSystem();
                    break;
                case ConsoleKey.Backspace:
                    stateManager.PopScreen();
                    break;
            }
        }

        private void MoveSelectionVertical(List<StarSystem> systems, int direction)
        {
            if (systems.Count == 0) return;
            int currentY = systems[selectedSystemIndex].Y;
            int bestIndex = -1;
            int bestDistX = int.MaxValue;
            for (int i = 0; i < systems.Count; i++)
            {
                if (i == selectedSystemIndex) continue;
                if (direction == -1 && systems[i].Y < currentY)
                {
                    int dx = Math.Abs(systems[i].X - systems[selectedSystemIndex].X);
                    if (dx < bestDistX)
                    {
                        bestDistX = dx;
                        bestIndex = i;
                    }
                }
                else if (direction == 1 && systems[i].Y > currentY)
                {
                    int dx = Math.Abs(systems[i].X - systems[selectedSystemIndex].X);
                    if (dx < bestDistX)
                    {
                        bestDistX = dx;
                        bestIndex = i;
                    }
                }
            }
            if (bestIndex != -1)
                selectedSystemIndex = bestIndex;
        }

        private void MoveSelectionHorizontal(List<StarSystem> systems, int direction)
        {
            if (systems.Count == 0) return;
            int currentX = systems[selectedSystemIndex].X;
            int bestIndex = -1;
            int bestDistY = int.MaxValue;
            for (int i = 0; i < systems.Count; i++)
            {
                if (i == selectedSystemIndex) continue;
                if (direction == -1 && systems[i].X < currentX)
                {
                    int dy = Math.Abs(systems[i].Y - systems[selectedSystemIndex].Y);
                    if (dy < bestDistY)
                    {
                        bestDistY = dy;
                        bestIndex = i;
                    }
                }
                else if (direction == 1 && systems[i].X > currentX)
                {
                    int dy = Math.Abs(systems[i].Y - systems[selectedSystemIndex].Y);
                    if (dy < bestDistY)
                    {
                        bestDistY = dy;
                        bestIndex = i;
                    }
                }
            }
            if (bestIndex != -1)
                selectedSystemIndex = bestIndex;
        }

        private void TryEnterSelectedSystem()
        {
            var systems = currentSector.Systems;
            if (selectedSystemIndex < 0 || selectedSystemIndex >= systems.Count) return;

            var targetSystem = systems[selectedSystemIndex];

            if (targetSystem == currentSystem)
            {
                stateManager.PushScreen(new LocalMapScreen(stateManager, uiManager, targetSystem));
                return;
            }

            if (!currentSystem.ConnectedSystems.Contains(targetSystem))
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Нет пути",
                    "Эта система не связана с текущей. Проложите маршрут через другие системы."));
                return;
            }

            string fromName = currentSystem.Name;
            string toName = targetSystem.Name;

            Action onJumpComplete = () =>
            {
                currentSystem = targetSystem;
                selectedSystemIndex = currentSector.Systems.IndexOf(currentSystem);
                Game.CurrentSystem = currentSystem;
                // Заменяем экран анимации на локальную карту
                stateManager.ReplaceScreen(new LocalMapScreen(stateManager, uiManager, targetSystem));
            };

            stateManager.PushScreen(new HyperJumpScreen(stateManager, uiManager, fromName, toName, onJumpComplete));
        }
    }
}