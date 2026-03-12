// Core/Game.cs
using System;
using System.Threading;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;
using ASTRANET.Screens;

namespace ASTRANET.Core;

public class Game
{
    private bool _isRunning = true;
    private UIManager _uiManager;
    private InputManager _inputManager;
    private ScreenManager _screenManager;

    public void Run()
    {
        Initialize();

        while (_isRunning)
        {
            float deltaTime = 0.05f;
            _inputManager.ProcessInput();
            _screenManager.Update(deltaTime);
            _uiManager.CheckResize();

            // Проверка смерти игрока
            try
            {
                var player = DI.Resolve<Player>();
                if (player.IsDead)
                {
                    var gameOverScreen = new GameOverScreen(_uiManager, _screenManager, _inputManager);
                    _screenManager.ChangeScreen(gameOverScreen);
                }
            }
            catch { }

            _uiManager.Clear();
            _screenManager.Render();
            _uiManager.Render();
            Thread.Sleep(45);
        }

        Shutdown();
    }

    private void Initialize()
    {
        _uiManager = new UIManager();
        _inputManager = new InputManager();
        _screenManager = new ScreenManager(_uiManager, _inputManager);

        DI.Register(_uiManager);
        DI.Register(_inputManager);
        DI.Register(_screenManager);

        _inputManager.Initialize(_screenManager);

        var mainMenu = new MainMenuScreen(_uiManager, _screenManager, _inputManager);
        _screenManager.ChangeScreen(mainMenu);
    }

    private void Shutdown() { }

    public void Exit() => _isRunning = false;
}