// Managers/InputManager.cs
using System;
using System.Collections.Generic;
using ASTRANET.Core;
using ASTRANET.Screens;
using ASTRANET.Models.Entities;

namespace ASTRANET.Managers;

public class InputManager
{
    private ScreenManager _screenManager;

    public void Initialize(ScreenManager screenManager)
    {
        _screenManager = screenManager;
    }

    public void ProcessInput()
    {
        while (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(true);
            if (HandleGlobalKeys(keyInfo))
                continue;

            _screenManager?.HandleInput(keyInfo);
        }
    }

    private bool HandleGlobalKeys(ConsoleKeyInfo keyInfo)
    {
        var uiManager = DI.Resolve<UIManager>();

        switch (keyInfo.Key)
        {
            case ConsoleKey.F5:
                try
                {
                    var saveLoad = new SaveLoadManager();
                    saveLoad.SaveGame("quicksave.json");
                    uiManager.ShowMessage("Быстрое сохранение выполнено");
                }
                catch (Exception ex)
                {
                    uiManager.ShowMessage($"Ошибка сохранения: {ex.Message}");
                }
                return true;

            case ConsoleKey.F9:
                try
                {
                    var saveLoad = new SaveLoadManager();
                    if (saveLoad.LoadGame("quicksave.json"))
                    {
                        uiManager.ShowMessage("Быстрая загрузка выполнена");
                        // После загрузки переходим на галактическую карту
                        var galaxyScreen = new GalaxyMapScreen(
                            DI.Resolve<UIManager>(),
                            DI.Resolve<ScreenManager>(),
                            this);
                        _screenManager?.ChangeScreen(galaxyScreen);
                    }
                    else
                    {
                        uiManager.ShowMessage("Нет сохранения или ошибка загрузки");
                    }
                }
                catch (Exception ex)
                {
                    uiManager.ShowMessage($"Ошибка загрузки: {ex.Message}");
                }
                return true;

            case ConsoleKey.Escape:
                // Пока ничего
                return true;
            case ConsoleKey.I:
                try
                {
                    var player = DI.Resolve<Player>();
                    if (player != null)
                    {
                        var invScreen = new InventoryScreen(
                            DI.Resolve<UIManager>(),
                            DI.Resolve<ScreenManager>(),
                            this);
                        _screenManager?.PushScreen(invScreen);
                    }
                }
                catch { }
                return true;
            case ConsoleKey.J:
                try
                {
                    var questScreen = new QuestLogScreen(
                        DI.Resolve<UIManager>(),
                        DI.Resolve<ScreenManager>(),
                        this);
                    _screenManager?.PushScreen(questScreen);
                }
                catch { }
                return true;
            case ConsoleKey.C:
                try
                {
                    var statsScreen = new StatsScreen(
                        DI.Resolve<UIManager>(),
                        DI.Resolve<ScreenManager>(),
                        this);
                    _screenManager?.PushScreen(statsScreen);
                }
                catch { }
                return true;
            case ConsoleKey.R:
                try
                {
                    var repScreen = new ReputationScreen(
                        DI.Resolve<UIManager>(),
                        DI.Resolve<ScreenManager>(),
                        this);
                    _screenManager?.PushScreen(repScreen);
                }
                catch { }
                return true;
            case ConsoleKey.U:
                try
                {
                    var upgradeScreen = new UpgradeScreen(
                        DI.Resolve<UIManager>(),
                        DI.Resolve<ScreenManager>(),
                        this);
                    _screenManager?.PushScreen(upgradeScreen);
                }
                catch { }
                return true;
            case ConsoleKey.Backspace:
                _screenManager?.PopScreen();
                return true;
            default:
                return false;
        }
    }
}