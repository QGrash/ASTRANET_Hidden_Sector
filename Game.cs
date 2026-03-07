using ASTRANET_Hidden_Sector.Entities;
using ASTRANET_Hidden_Sector.Screens;
using System;
using System.Threading;
using ASTRANET_Hidden_Sector.Entities.Dialogue;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.World;
using ASTRANET_Hidden_Sector.Entities.Trade;

namespace ASTRANET_Hidden_Sector.Core
{
    public class Game
    {
        private GameStateManager stateManager;
        private InputHandler inputHandler;
        private UIManager uiManager;
        private bool isRunning;

        public static Player? CurrentPlayer { get; set; }
        public static List<Sector>? CurrentWorld { get; set; }
        public static Sector? CurrentSector { get; set; }
        public static StarSystem? CurrentSystem { get; set; }

        public Game()
        {
            uiManager = new UIManager();
            stateManager = new GameStateManager(uiManager);
            inputHandler = new InputHandler(stateManager);

            DialogueLoader.LoadAll();
            TradeGoodsLoader.LoadAll();

            isRunning = true;
        }

        public void Run()
        {
            stateManager.PushScreen(new MainMenuScreen(stateManager, uiManager));

            while (isRunning)
            {
                try
                {
                    inputHandler.ProcessInput();
                    stateManager.Update(0.05f);
                    uiManager.Clear();
                    stateManager.Render();
                    uiManager.Render();
                    Thread.Sleep(45);
                }
                catch (Exception ex)
                {
                    // Здесь можно записать в лог, но не выводим на экран
                   
                }
            }
        }

        public void Exit()
        {
            isRunning = false;
        }
    }
}