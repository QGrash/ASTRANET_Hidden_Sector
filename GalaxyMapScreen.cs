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
                int screenX = 10 + (int)((sector.X / 100.0) * 60);
                int screenY = 5 + (int)((sector.Y / 100.0) * 15);

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

                uiManager.SetCursorPosition(screenX, screenY);
                uiManager.Write(symbol, color);

                uiManager.SetCursorPosition(screenX + 2, screenY);
                uiManager.Write(sector.Name, sector.IsLocked ? ConsoleColor.DarkRed : ConsoleColor.White);
            }

            DrawInfoPanel();

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("↑/↓ - выбор сектора, Enter - войти (если доступен), Esc - назад", ConsoleColor.DarkGray);
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

        private void DrawConnections()
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                var s1 = sectors[i];
                int x1 = 10 + (int)((s1.X / 100.0) * 60);
                int y1 = 5 + (int)((s1.Y / 100.0) * 15);

                foreach (var s2 in s1.ConnectedSectors)
                {
                    int x2 = 10 + (int)((s2.X / 100.0) * 60);
                    int y2 = 5 + (int)((s2.Y / 100.0) * 15);
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
                    int sxPos = 10 + (int)((s.X / 100.0) * 60);
                    int syPos = 5 + (int)((s.Y / 100.0) * 15);
                    if (x1 == sxPos && y1 == syPos)
                    {
                        isSector = true;
                        break;
                    }
                }
                if (!isSector)
                {
                    uiManager.SetCursorPosition(x1, y1);
                    uiManager.Write("·", color);
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
            uiManager.SetCursorPosition(x - 1, y - 1);
            uiManager.Write("┌─┐", color);
            uiManager.SetCursorPosition(x - 1, y);
            uiManager.Write("│", color);
            uiManager.SetCursorPosition(x + 1, y);
            uiManager.Write("│", color);
            uiManager.SetCursorPosition(x - 1, y + 1);
            uiManager.Write("└─┘", color);
        }

        private void DrawInfoPanel()
        {
            int infoX = 70;
            int infoY = 5;
            var selected = sectors[selectedSectorIndex];

            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write("=== ИНФОРМАЦИЯ ===", ConsoleColor.Yellow);
            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write($"Текущий сектор: {Game.CurrentSector?.Name ?? "Неизвестно"}", ConsoleColor.Green);
            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write($"Выбранный сектор: {selected.Name}", selected.IsLocked ? ConsoleColor.Red : ConsoleColor.Cyan);
            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write($"Статус: {(selected.IsLocked ? "ЗАКРЫТ" : "ОТКРЫТ")}", selected.IsLocked ? ConsoleColor.Red : ConsoleColor.Green);
            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write($"Систем: {selected.Systems.Count}", ConsoleColor.White);
            uiManager.SetCursorPosition(infoX, infoY++);
            uiManager.Write("Доступные сектора из текущего:", ConsoleColor.Cyan);
            if (Game.CurrentSector != null)
            {
                foreach (var conn in Game.CurrentSector.ConnectedSectors)
                {
                    uiManager.SetCursorPosition(infoX + 2, infoY++);
                    uiManager.Write(conn.Name, conn.IsLocked ? ConsoleColor.DarkRed : ConsoleColor.White);
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

            Game.CurrentSector = sector;
            Game.CurrentSystem = null;

            stateManager.PushScreen(new GlobalMapScreen(stateManager, uiManager, sector, startSystem));
        }
    }
}