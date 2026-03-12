// Managers/ScreenManager.cs
using ASTRANET.Core;
using ASTRANET.Screens;
using System.Collections.Generic;

namespace ASTRANET.Managers;

public class ScreenManager
{
    private readonly Stack<BaseScreen> _screenStack = new();
    private readonly UIManager _uiManager;
    private readonly InputManager _inputManager;

    public ScreenManager(UIManager uiManager, InputManager inputManager)
    {
        _uiManager = uiManager;
        _inputManager = inputManager;
    }

    public void PushScreen(BaseScreen screen)
    {
        if (_screenStack.Count > 0)
            _screenStack.Peek().OnExit();
        _screenStack.Push(screen);
        screen.OnEnter();
        _uiManager.ForceFullRedraw();
    }

    public void PopScreen()
    {
        if (_screenStack.Count > 0)
        {
            var screen = _screenStack.Pop();
            screen.OnExit();
        }

        if (_screenStack.Count > 0)
        {
            _screenStack.Peek().OnEnter();
        }
        else
        {
            // Если стек пуст – возвращаемся в главное меню (чтобы не было чёрного экрана)
            var mainMenu = new MainMenuScreen(_uiManager, this, _inputManager);
            ChangeScreen(mainMenu);
            return;
        }

        _uiManager.ForceFullRedraw();
    }

    public void ChangeScreen(BaseScreen screen)
    {
        while (_screenStack.Count > 0)
            _screenStack.Pop().OnExit();
        _screenStack.Push(screen);
        screen.OnEnter();
        _uiManager.ForceFullRedraw();
    }

    public void Update(float deltaTime)
    {
        if (_screenStack.Count > 0)
            _screenStack.Peek().Update(deltaTime);
    }

    public void Render()
    {
        if (_screenStack.Count > 0)
        {
            _screenStack.Peek().Render();
            _uiManager.DrawMessage();
        }
    }

    public void HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (_screenStack.Count > 0)
            _screenStack.Peek().HandleInput(keyInfo);
    }
}