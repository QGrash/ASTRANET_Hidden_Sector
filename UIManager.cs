using System;
using System.Text;

namespace ASTRANET_Hidden_Sector.Core
{
    public class UIManager
    {
        private char[,] currentScreen;
        private ConsoleColor[,] currentColors;
        private char[,] previousScreen;
        private ConsoleColor[,] previousColors;
        private int width, height;
        private bool fullRedraw;

        public UIManager()
        {
            width = Console.WindowWidth;
            height = Console.WindowHeight;
            currentScreen = new char[width, height];
            currentColors = new ConsoleColor[width, height];
            previousScreen = new char[width, height];
            previousColors = new ConsoleColor[width, height];
            fullRedraw = true;
        }

        public void Clear()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    currentScreen[x, y] = ' ';
                    currentColors[x, y] = ConsoleColor.Black;
                }
        }

        public void SetPixel(int x, int y, char c, ConsoleColor color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                currentScreen[x, y] = c;
                currentColors[x, y] = color;
            }
        }

        public void ForceFullRedraw()
        {
            fullRedraw = true;
        }

        public void Render()
        {
            if (fullRedraw)
            {
                // Полная перерисовка: выводим всё, что есть в current
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = currentColors[x, y];
                        Console.Write(currentScreen[x, y]);
                    }
                }
                // Копируем current в previous
                Array.Copy(currentScreen, previousScreen, currentScreen.Length);
                Array.Copy(currentColors, previousColors, currentColors.Length);
                fullRedraw = false;
            }
            else
            {
                // Инкрементальная отрисовка
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (currentScreen[x, y] != previousScreen[x, y] ||
                            currentColors[x, y] != previousColors[x, y])
                        {
                            Console.SetCursorPosition(x, y);
                            Console.ForegroundColor = currentColors[x, y];
                            Console.Write(currentScreen[x, y]);
                        }
                    }
                }
                // Копируем current в previous
                Array.Copy(currentScreen, previousScreen, currentScreen.Length);
                Array.Copy(currentColors, previousColors, currentColors.Length);
            }
        }
    }
}