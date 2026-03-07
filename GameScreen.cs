using System;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.Screens
{
    public abstract class GameScreen
    {
        protected GameStateManager stateManager;
        protected UIManager uiManager;

        public GameScreen(GameStateManager stateManager, UIManager uiManager)
        {
            this.stateManager = stateManager;
            this.uiManager = uiManager;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void Update(float deltaTime) { }
        public abstract void Render();
        public abstract void HandleInput(ConsoleKeyInfo key);
    }
}