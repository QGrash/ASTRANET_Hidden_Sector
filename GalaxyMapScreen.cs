using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.World;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class GalaxyMapScreen : GameScreen
    {
        private List<Sector> sectors;
        private Sector currentSector;
        private int selectedSectorIndex = 0;
        private Random rand = new Random();

        private struct Star
        {
            public int X;
            public int Y;
            public char Symbol;
            public ConsoleColor Color;
        }
        private Star[] stars;
        private int starOffset = 0;

        // Фиксированные отступы
        private const int MAP_LEFT = 5;
        private const int MAP_TOP = 5;
        private const int MAP_WIDTH = 60;
        private const int MAP_HEIGHT = 20;

        public Sector CurrentSector => currentSector;

        public void UpdateCurrentSector(Sector sector)
        {
            currentSector = sector;
        }

        public GalaxyMapScreen(GameStateManager stateManager, UIManager uiManager, List<Sector> sectors, Sector startSector)
            : base(stateManager, uiManager)
        {
            this.sectors = sectors;
            this.currentSector = startSector;
            if (Game.CurrentSector == null)
                Game.CurrentSector = startSector;
            selectedSectorIndex = sectors.IndexOf(startSector);
            if (selectedSectorIndex == -1) selectedSectorIndex = 0;

            Game.CurrentSystem = null;

            stars = new Star[50];
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].X = rand.Next(0, Console.WindowWidth);
                stars[i].Y = rand.Next(0, Console.WindowHeight);
                stars[i].Symbol = rand.Next(3) == 0 ? '*' : '.';
                stars[i].Color = rand.Next(2) == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray;
            }
        }

        public override void Render()
        {
            uiManager.Clear();

            DrawStars();
            DrawConnections();

            foreach (var sector in sectors)
            {
                int screenX = MAP_LEFT + (int)((sector.X / 100.0) * MAP_WIDTH);
                int screenY = MAP_TOP + (int)((sector.Y / 100.0) * MAP_HEIGHT);

                string symbol = sector.IsLocked ? "🔒" : "⏣";
                ConsoleColor color = sector.IsLocked ? ConsoleColor.Red : ConsoleColor.Cyan;

                if (sector == sectors[selectedSectorIndex])
                {
                    DrawSelectionBox(screenX, screenY, ConsoleColor.Yellow);
                }
                if (sector == Game.CurrentSector && sector != sectors[selectedSectorIndex])
                {
                    DrawSelectionBox(screenX, screenY, ConsoleColor.Green);
                }

                for (int i = 0; i < symbol.Length; i++)
                    uiManager.SetPixel(screenX + i, screenY, symbol[i], color);

                for (int i = 0; i < sector.Name.Length; i++)
                    uiManager.SetPixel(screenX + 2 + i, screenY, sector.Name[i],
                        sector.IsLocked ? ConsoleColor.DarkRed : ConsoleColor.White);
            }

            DrawInfoPanel();

            string hint = "↑/↓ - выбор сектора, Enter - войти (если доступен), Esc - назад";
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

        private void DrawConnections()
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                var s1 = sectors[i];
                int x1 = MAP_LEFT + (int)((s1.X / 100.0) * MAP_WIDTH);
                int y1 = MAP_TOP + (int)((s1.Y / 100.0) * MAP_HEIGHT);

                foreach (var s2 in s1.ConnectedSectors)
                {
                    int x2 = MAP_LEFT + (int)((s2.X / 100.0) * MAP_WIDTH);
                    int y2 = MAP_TOP + (int)((s2.Y / 100.0) * MAP_HEIGHT);
                    DrawLine(x1, y1, x2, y2, s2.IsLocked ? ConsoleColor.DarkRed : ConsoleColor.DarkGray);
                }
            }
        }

        private void DrawLine(int x1, int y1, int x2, int y2, ConsoleColor color)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                bool isSector = false;
                foreach (var s in sectors)
                {
                    int sxPos = MAP_LEFT + (int)((s.X / 100.0) * MAP_WIDTH);
                    int syPos = MAP_TOP + (int)((s.Y / 100.0) * MAP_HEIGHT);
                    if (x1 == sxPos && y1 == syPos)
                    {
                        isSector = true;
                        break;
                    }
                }
                if (!isSector)
                {
                    uiManager.SetPixel(x1, y1, '·', color);
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

        private void DrawSelectionBox(int x, int y, ConsoleColor color)
        {
            uiManager.SetPixel(x - 1, y - 1, '┌', color);
            uiManager.SetPixel(x, y - 1, '─', color);
            uiManager.SetPixel(x + 1, y - 1, '┐', color);
            uiManager.SetPixel(x - 1, y, '│', color);
            uiManager.SetPixel(x + 1, y, '│', color);
            uiManager.SetPixel(x - 1, y + 1, '└', color);
            uiManager.SetPixel(x, y + 1, '─', color);
            uiManager.SetPixel(x + 1, y + 1, '┘', color);
        }

        private void DrawInfoPanel()
        {
            int infoX = MAP_LEFT + MAP_WIDTH + 5;
            int infoY = MAP_TOP;
            var selected = sectors[selectedSectorIndex];

            string line = "=== ИНФОРМАЦИЯ ===";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Yellow);
            infoY++;

            line = $"Текущий сектор: {Game.CurrentSector?.Name ?? "Неизвестно"}";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Green);
            infoY++;

            line = $"Выбранный сектор: {selected.Name}";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], selected.IsLocked ? ConsoleColor.Red : ConsoleColor.Cyan);
            infoY++;

            line = $"Статус: {(selected.IsLocked ? "ЗАКРЫТ" : "ОТКРЫТ")}";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], selected.IsLocked ? ConsoleColor.Red : ConsoleColor.Green);
            infoY++;

            line = $"Систем: {selected.Systems.Count}";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.White);
            infoY++;

            line = "Доступные сектора из текущего:";
            for (int i = 0; i < line.Length; i++)
                uiManager.SetPixel(infoX + i, infoY, line[i], ConsoleColor.Cyan);
            infoY++;

            if (Game.CurrentSector != null)
            {
                foreach (var conn in Game.CurrentSector.ConnectedSectors)
                {
                    line = $"  {conn.Name}";
                    for (int i = 0; i < line.Length; i++)
                        uiManager.SetPixel(infoX + i, infoY, line[i], conn.IsLocked ? ConsoleColor.DarkRed : ConsoleColor.White);
                    infoY++;
                }
            }
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                    selectedSectorIndex = (selectedSectorIndex - 1 + sectors.Count) % sectors.Count;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                    selectedSectorIndex = (selectedSectorIndex + 1) % sectors.Count;
                    break;
                case ConsoleKey.Enter:
                    TryEnterSector();
                    break;
                case ConsoleKey.Escape:
                    stateManager.PopScreen();
                    break;
            }
        }

        private void TryEnterSector()
        {
            var sector = sectors[selectedSectorIndex];
            if (sector.IsLocked)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Сектор закрыт",
                    "Этот сектор пока недоступен. Найдите фрагменты координат."));
                return;
            }

            if (Game.CurrentSector == null || !Game.CurrentSector.ConnectedSectors.Contains(sector) && sector != Game.CurrentSector)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Перемещение невозможно",
                    "Этот сектор не связан с текущим."));
                return;
            }

            if (sector.Systems.Count == 0)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Ошибка",
                    "В секторе нет систем!"));
                return;
            }

            int randomSystemIndex = rand.Next(sector.Systems.Count);
            var startSystem = sector.Systems[randomSystemIndex];

            string fromName = Game.CurrentSector?.Name ?? "Неизвестно";
            string toName = sector.Name;

            Action onJumpComplete = () =>
            {
                Game.CurrentSector = sector;
                Game.CurrentSystem = null;
                // Заменяем экран анимации на карту сектора
                stateManager.ReplaceScreen(new GlobalMapScreen(stateManager, uiManager, sector, startSystem));
            };

            stateManager.PushScreen(new HyperJumpScreen(stateManager, uiManager, fromName, toName, onJumpComplete));
        }
    }
}