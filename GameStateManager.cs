using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.Core
{
    public class GameStateManager
    {
        private Stack<GameScreen> screens = new Stack<GameScreen>();
        public UIManager UIManager { get; }

        public GameStateManager(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        public void PushScreen(GameScreen screen)
        {
            if (screens.Count > 0)
                screens.Peek().Pause();
            screens.Push(screen);
            screen.Enter();
        }

        public void PopScreen()
        {
            if (screens.Count > 0)
            {
                var screen = screens.Pop();
                screen.Exit();
            }
            if (screens.Count > 0)
            {
                Console.Clear(); // принудительная очистка
                UIManager.ForceFullRedraw();
                var newScreen = screens.Peek();
                newScreen.Resume();
                if (newScreen is GalaxyMapScreen galaxyScreen && Game.CurrentSector != null)
                {
                    galaxyScreen.UpdateCurrentSector(Game.CurrentSector);
                }
            }
        }

        public void ChangeScreen(GameScreen screen)
        {
            while (screens.Count > 0)
                screens.Pop().Exit();
            PushScreen(screen);
        }

        // Заменяет текущий верхний экран на новый (без удаления нижних)
        public void ReplaceScreen(GameScreen newScreen)
        {
            if (screens.Count > 0)
            {
                var oldScreen = screens.Pop();
                oldScreen.Exit();
            }
            PushScreen(newScreen);
        }

        public void HandleInput(ConsoleKeyInfo key)
        {
            if (screens.Count > 0)
                screens.Peek().HandleInput(key);
        }

        public void Update(float deltaTime)
        {
            if (screens.Count > 0)
                screens.Peek().Update(deltaTime);
        }

        public void Render()
        {
            if (screens.Count > 0)
                screens.Peek().Render();
        }

        public T? GetCurrentScreen<T>() where T : GameScreen
        {
            if (screens.Count > 0 && screens.Peek() is T screen)
                return screen;
            return null;
        }

        public T? GetPreviousScreen<T>() where T : GameScreen
        {
            if (screens.Count < 2)
                return null;
            var array = screens.ToArray();
            if (array.Length >= 2 && array[1] is T screen)
                return screen;
            return null;
        }
    }
}