using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.World;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class GlobalMapScreen : GameScreen
    {
        public Sector CurrentSector => currentSector;
        private Sector currentSector;
        private StarSystem currentSystem;
        private int selectedSystemIndex;
        public StarSystem CurrentSystem => currentSystem;

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
            DrawSectorFrame(currentSector.Name, currentSector.IsLocked);

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
                    uiManager.SetCursorPosition(x - 1, y);
                    uiManager.Write("[", ConsoleColor.Yellow);
                    uiManager.SetCursorPosition(x + 1, y);
                    uiManager.Write("]", ConsoleColor.Yellow);
                }

                uiManager.SetCursorPosition(x, y);
                uiManager.Write(symbol.ToString(), color);
            }

            if (currentSystem != null)
            {
                int curX = MAP_LEFT + (int)((currentSystem.X / 100.0) * MAP_WIDTH);
                int curY = MAP_TOP + (int)((currentSystem.Y / 100.0) * MAP_HEIGHT);
                uiManager.SetCursorPosition(curX - 1, curY - 1);
                uiManager.Write("[", ConsoleColor.Green);
                uiManager.SetCursorPosition(curX + 1, curY);
                uiManager.Write("]", ConsoleColor.Green);
            }

            DrawInfoPanel(screenPositions);

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("↑/↓ - выбор системы, Enter - переход (если есть связь), G - карта галактики, I - инвентарь, C - персонаж, B - корабль, J - квесты, Backspace - назад, ESC - меню", ConsoleColor.DarkGray);
        }

        private void DrawStars()
        {
            starOffset = (starOffset + 1) % Console.WindowHeight;
            for (int i = 0; i < stars.Length; i++)
            {
                int y = (stars[i].Y + starOffset) % Console.WindowHeight;
                uiManager.SetCursorPosition(stars[i].X, y);
                uiManager.Write(stars[i].Symbol.ToString(), stars[i].Color);
            }
        }

        private void DrawSectorFrame(string sectorName, bool isLocked)
        {
            for (int x = MAP_LEFT - 1; x <= MAP_LEFT + MAP_WIDTH; x++)
            {
                uiManager.SetCursorPosition(x, MAP_TOP - 1);
                uiManager.Write("─", ConsoleColor.DarkGray);
                uiManager.SetCursorPosition(x, MAP_TOP + MAP_HEIGHT);
                uiManager.Write("─", ConsoleColor.DarkGray);
            }
            for (int y = MAP_TOP - 1; y <= MAP_TOP + MAP_HEIGHT; y++)
            {
                uiManager.SetCursorPosition(MAP_LEFT - 1, y);
                uiManager.Write("│", ConsoleColor.DarkGray);
                uiManager.SetCursorPosition(MAP_LEFT + MAP_WIDTH, y);
                uiManager.Write("│", ConsoleColor.DarkGray);
            }
            uiManager.SetCursorPosition(MAP_LEFT - 1, MAP_TOP - 1);
            uiManager.Write("┌", ConsoleColor.DarkGray);
            uiManager.SetCursorPosition(MAP_LEFT + MAP_WIDTH, MAP_TOP - 1);
            uiManager.Write("┐", ConsoleColor.DarkGray);
            uiManager.SetCursorPosition(MAP_LEFT - 1, MAP_TOP + MAP_HEIGHT);
            uiManager.Write("└", ConsoleColor.DarkGray);
            uiManager.SetCursorPosition(MAP_LEFT + MAP_WIDTH, MAP_TOP + MAP_HEIGHT);
            uiManager.Write("┘", ConsoleColor.DarkGray);

            uiManager.SetCursorPosition(MAP_LEFT, MAP_TOP - 2);
            uiManager.Write($"Сектор: ", ConsoleColor.Cyan);
            if (isLocked)
                uiManager.Write(sectorName, ConsoleColor.Red);
            else
                uiManager.Write(sectorName, ConsoleColor.Green);
        }

        private void DrawConnections(List<(int x, int y, StarSystem sys)> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                var (x1, y1, sys1) = positions[i];
                foreach (var conn in sys1.ConnectedSystems)
                {
                    var connPos = positions.FirstOrDefault(p => p.sys == conn);
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
                    uiManager.SetCursorPosition(x1, y1);
                    uiManager.Write("·", ConsoleColor.DarkGray);
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

            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write("=== ИНФОРМАЦИЯ ===", ConsoleColor.Yellow);

            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write($"Текущий сектор: ", ConsoleColor.White);
            uiManager.Write(currentSector.Name, currentSector.IsLocked ? ConsoleColor.Red : ConsoleColor.Green);

            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write($"Текущая система: {currentSystem.Name}", ConsoleColor.Green);

            if (selectedSystemIndex >= 0 && selectedSystemIndex < positions.Count)
            {
                var sys = positions[selectedSystemIndex].sys;
                infoY++;
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write("Выбранная система:", ConsoleColor.Yellow);
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write($"Название: {sys.Name}", ConsoleColor.White);
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write($"Тип: {sys.Type}", GetSystemColor(sys.Type));
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write($"Координаты: {sys.X}, {sys.Y}", ConsoleColor.DarkGray);
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write("Связи:", ConsoleColor.Cyan);
                foreach (var conn in sys.ConnectedSystems)
                {
                    uiManager.SetCursorPosition(infoX + 2, infoY++);
                    uiManager.Write(conn.Name, ConsoleColor.Gray);
                }
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write($"Соединена с текущей: {(currentSystem.ConnectedSystems.Contains(sys) ? "ДА" : "НЕТ")}",
                    currentSystem.ConnectedSystems.Contains(sys) ? ConsoleColor.Green : ConsoleColor.Red);
            }
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            var systems = currentSector.Systems;

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (systems.Count > 0)
                        selectedSystemIndex = (selectedSystemIndex - 1 + systems.Count) % systems.Count;
                    break;
                case ConsoleKey.DownArrow:
                    if (systems.Count > 0)
                        selectedSystemIndex = (selectedSystemIndex + 1) % systems.Count;
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    // Ничего не делаем, можно оставить для совместимости или игнорировать
                    break;
                case ConsoleKey.G:
                    Game.CurrentSystem = null; // <-- добавить
                    stateManager.PopScreen();
                    break;
                case ConsoleKey.Backspace:
                    Game.CurrentSystem = null; // <-- добавить
                    stateManager.PopScreen();
                    break;
                case ConsoleKey.Enter:
                    TryEnterSelectedSystem();
                    break;
            }
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

            currentSystem = targetSystem;
            selectedSystemIndex = currentSector.Systems.IndexOf(currentSystem);

            stateManager.PushScreen(new LocalMapScreen(stateManager, uiManager, targetSystem));
        }
    }
}