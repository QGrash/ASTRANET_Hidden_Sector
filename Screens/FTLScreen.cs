// Screens/FTLScreen.cs
using System;
using System.Collections.Generic;
using System.Threading;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;

namespace ASTRANET.Screens;

public class FTLScreen : BaseScreen
{
    private readonly WorldManager _worldManager;
    private readonly Player _player;
    private readonly int _targetSystemIndex;
    private int _progress = 0;
    private const int MaxProgress = 20;

    public FTLScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager, int targetSystemIndex)
        : base(uiManager, inputManager, screenManager)
    {
        _worldManager = DI.Resolve<WorldManager>();
        _player = DI.Resolve<Player>();
        _targetSystemIndex = targetSystemIndex;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        System.Threading.Tasks.Task.Run(() => AnimateJump());
    }

    private void AnimateJump()
    {
        for (int i = 0; i <= MaxProgress; i++)
        {
            _progress = i;
            Thread.Sleep(100);
        }

        _worldManager.TryEnterSystem(_targetSystemIndex, 0);
        var localMapScreen = new LocalMapScreen(UIManager, ScreenManager, InputManager);
        ScreenManager.PopScreen();
        ScreenManager.PushScreen(localMapScreen);
    }

    public override void Render()
    {
        UIManager.Clear();

        int centerX = UIManager.Width / 2;
        int centerY = UIManager.Height / 2;

        UIManager.DrawString(centerX - 10, centerY - 2, "ПРЫЖОК В ГИПЕРПРОСТРАНСТВО", ConsoleColor.Cyan);

        string bar = new string('#', _progress) + new string('-', MaxProgress - _progress);
        UIManager.DrawString(centerX - 10, centerY, $"[{bar}]", ConsoleColor.Green);
        UIManager.DrawString(centerX - 5, centerY + 1, $"{_progress * 5}%", ConsoleColor.Yellow);

        UIManager.DrawString(centerX - 10, UIManager.Height - 2, "Пожалуйста, подождите...", ConsoleColor.Gray);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo) => true;
}