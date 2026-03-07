using System;
using System.Text;

namespace ASTRANET_Hidden_Sector.Core
{
    public class UIManager
    {
        private StringBuilder buffer;

        public UIManager()
        {
            buffer = new StringBuilder();
        }

        // Очищает экран и буфер
        public void Clear()
        {
            buffer.Clear();
            // ANSI-код: очистить весь экран и переместить курсор в (0,0)
            buffer.Append("\x1b[2J\x1b[H");
        }

        // Устанавливает позицию для следующего вывода (будет использовано в следующем Write)
        public void SetCursorPosition(int left, int top)
        {
            // Добавляем код перемещения курсора; он будет применён при следующем Write
            buffer.Append($"\x1b[{top + 1};{left + 1}H");
        }

        // Преобразует ConsoleColor в ANSI-код цвета текста
        private string GetAnsiColorCode(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => "30",
                ConsoleColor.DarkRed => "31",
                ConsoleColor.DarkGreen => "32",
                ConsoleColor.DarkYellow => "33",
                ConsoleColor.DarkBlue => "34",
                ConsoleColor.DarkMagenta => "35",
                ConsoleColor.DarkCyan => "36",
                ConsoleColor.Gray => "37",
                ConsoleColor.DarkGray => "90",
                ConsoleColor.Red => "91",
                ConsoleColor.Green => "92",
                ConsoleColor.Yellow => "93",
                ConsoleColor.Blue => "94",
                ConsoleColor.Magenta => "95",
                ConsoleColor.Cyan => "96",
                ConsoleColor.White => "97",
                _ => "39" // default
            };
        }

        // Записывает текст в буфер с опциональным цветом
        public void Write(string text, ConsoleColor? color = null)
        {
            if (color.HasValue)
            {
                buffer.Append($"\x1b[{GetAnsiColorCode(color.Value)}m");
            }
            buffer.Append(text);
            if (color.HasValue)
            {
                buffer.Append("\x1b[39m"); // сброс цвета текста
            }
        }

        public void WriteLine(string text, ConsoleColor? color = null)
        {
            Write(text + "\n", color);
        }

        // Выводит всё накопленное в буфере на экран и очищает буфер
        public void Render()
        {
            Console.Write(buffer.ToString());
            buffer.Clear();
        }
    }
}