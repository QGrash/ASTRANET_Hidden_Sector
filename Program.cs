using System;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.Title = "ASTRANET: Hidden Sector";
            Console.CursorVisible = false;

            Game game = new Game();
            game.Run();
        }
    }
}