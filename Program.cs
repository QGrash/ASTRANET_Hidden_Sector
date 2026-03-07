using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "ASTRANET: Hidden Sector";
            Console.CursorVisible = false;

            Game game = new Game();
            game.Run();
        }
    }
}