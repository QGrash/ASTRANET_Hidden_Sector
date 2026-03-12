using System.Collections.Generic;
using ASTRANET.Core;      // для UIManager
using ASTRANET.Managers;   // для InputManager и ScreenManager

namespace ASTRANET.Screens;

public abstract class BaseScreen
{
    protected UIManager UIManager { get; }
    protected InputManager InputManager { get; }
    protected ScreenManager ScreenManager { get; }

    protected BaseScreen(UIManager uiManager, InputManager inputManager, ScreenManager screenManager)
    {
        UIManager = uiManager;
        InputManager = inputManager;
        ScreenManager = screenManager;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Update(float deltaTime) { }
    public abstract void Render();
    public virtual bool HandleInput(ConsoleKeyInfo keyInfo) => false;
    public virtual List<(string text, ConsoleColor color)> GetHints() => new();
}