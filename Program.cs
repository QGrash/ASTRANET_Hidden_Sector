// Program.cs
using System;
using System.Text;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Utils;

namespace ASTRANET;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Title = "ASTRANET: Hidden Sector";

        try
        {
            Console.SetWindowSize(120, 40);
            Console.SetBufferSize(120, 40);
        }
        catch { }

        RandomManager.SetSeed(12345);

        var itemManager = new ItemManager();
        itemManager.LoadAllPrototypes();
        DI.Register(itemManager);

        var worldManager = new WorldManager();
        worldManager.GenerateGalaxy(12345);
        DI.Register(worldManager);

        var dialogueManager = new DialogueManager();
        dialogueManager.LoadAllDialogues();
        DI.Register(dialogueManager);

        var questManager = new QuestManager();
        questManager.LoadAllPrototypes();
        DI.Register(questManager);

        var reputationManager = new ReputationManager();
        DI.Register(reputationManager);

        var shopManager = new ShopManager();
        shopManager.LoadAllShops();
        DI.Register(shopManager);

        var techManager = new TechManager();
        DI.Register(techManager);

        var game = new Game();
        game.Run();

        Console.CursorVisible = true;
    }
}