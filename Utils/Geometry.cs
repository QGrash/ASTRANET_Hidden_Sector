// Utils/Geometry.cs
using System;
using System.Collections.Generic;

namespace ASTRANET.Utils;

public static class Geometry
{
    public static double Distance(int x1, int y1, int x2, int y2)
    {
        int dx = x1 - x2;
        int dy = y1 - y2;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static List<(int x, int y)> BresenhamCircle(int centerX, int centerY, int radius, bool dashed = false)
    {
        var points = new List<(int, int)>();
        int x = 0;
        int y = radius;
        int d = 3 - 2 * radius;

        void AddPoints(int cx, int cy, int x, int y)
        {
            points.Add((cx + x, cy + y));
            points.Add((cx - x, cy + y));
            points.Add((cx + x, cy - y));
            points.Add((cx - x, cy - y));
            points.Add((cx + y, cy + x));
            points.Add((cx - y, cy + x));
            points.Add((cx + y, cy - x));
            points.Add((cx - y, cy - x));
        }

        AddPoints(centerX, centerY, x, y);
        while (y >= x)
        {
            x++;
            if (d > 0)
            {
                y--;
                d = d + 4 * (x - y) + 10;
            }
            else
            {
                d = d + 4 * x + 6;
            }
            AddPoints(centerX, centerY, x, y);
        }

        var hashSet = new HashSet<(int, int)>(points);
        points = new List<(int, int)>(hashSet);

        if (dashed)
        {
            points.RemoveAll(p => (p.Item1 + p.Item2) % 2 != 0);
        }

        return points;
    }
}