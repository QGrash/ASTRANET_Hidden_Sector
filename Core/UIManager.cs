// Core/UIManager.cs
using System;
using System.Collections.Generic;

namespace ASTRANET.Core;

public class UIManager
{
    private char[,] _currentBuffer;
    private char[,] _previousBuffer;
    private ConsoleColor[,] _currentColorBuffer;
    private ConsoleColor[,] _previousColorBuffer;

    public int Width { get; private set; }
    public int Height { get; private set; }

    private string _message = "";
    private int _messageTimer = 0;
    private const int MessageDuration = 60; // примерно 3 секунды при 20 fps

    public UIManager()
    {
        Width = Console.WindowWidth;
        Height = Console.WindowHeight;
        Resize(Width, Height);
    }

    public void CheckResize()
    {
        int newWidth = Console.WindowWidth;
        int newHeight = Console.WindowHeight;
        if (newWidth != Width || newHeight != Height)
        {
            Resize(newWidth, newHeight);
        }
    }

    public void Resize(int width, int height)
    {
        if (width < 1) width = 1;
        if (height < 1) height = 1;

        Width = width;
        Height = height;

        try
        {
            Console.BufferWidth = width;
            Console.BufferHeight = height;
        }
        catch { }

        _currentBuffer = new char[height, width];
        _previousBuffer = new char[height, width];
        _currentColorBuffer = new ConsoleColor[height, width];
        _previousColorBuffer = new ConsoleColor[height, width];

        Clear();
        ForceFullRedraw();
    }

    public void SetPixel(int x, int y, char c, ConsoleColor color)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;
        _currentBuffer[y, x] = c;
        _currentColorBuffer[y, x] = color;
    }

    public char GetPixel(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return ' ';
        return _currentBuffer[y, x];
    }

    public void Clear()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _currentBuffer[y, x] = ' ';
    }

    public void ForceFullRedraw()
    {
        Array.Copy(_currentBuffer, _previousBuffer, _currentBuffer.Length);
        Array.Copy(_currentColorBuffer, _previousColorBuffer, _currentColorBuffer.Length);
        Console.Clear();
    }

    public void Render()
    {
        // Обновляем таймер сообщения
        if (_messageTimer > 0)
        {
            _messageTimer--;
        }

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_currentBuffer[y, x] != _previousBuffer[y, x] ||
                    _currentColorBuffer[y, x] != _previousColorBuffer[y, x])
                {
                    try
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = _currentColorBuffer[y, x];
                        Console.Write(_currentBuffer[y, x]);
                    }
                    catch { }
                    _previousBuffer[y, x] = _currentBuffer[y, x];
                    _previousColorBuffer[y, x] = _currentColorBuffer[y, x];
                }
            }
        }
    }

    public void DrawString(int x, int y, string text, ConsoleColor color)
    {
        for (int i = 0; i < text.Length; i++)
            SetPixel(x + i, y, text[i], color);
    }

    public void DrawHints(List<(string text, ConsoleColor color)> hints)
    {
        int y = Height - 2;
        int x = 2;
        foreach (var hint in hints)
        {
            DrawString(x, y, hint.text, hint.color);
            x += hint.text.Length + 2;
        }
    }

    public void ShowMessage(string message)
    {
        _message = message;
        _messageTimer = MessageDuration;
    }

    public void DrawMessage()
    {
        if (_messageTimer > 0 && !string.IsNullOrEmpty(_message))
        {
            int x = (Width - _message.Length) / 2;
            int y = Height / 2 - 2;
            DrawString(x, y, _message, ConsoleColor.Yellow);
        }
    }
}