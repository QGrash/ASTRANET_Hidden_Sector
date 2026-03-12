// Managers/TechManager.cs
using System;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Models.Entities;

namespace ASTRANET.Managers;

public class TechManager
{
    public void AddTechPoints(int amount)
    {
        try
        {
            var player = DI.Resolve<Player>();
            player.TechPoints += amount;
        }
        catch
        {
            // Игрок ещё не создан — игнорируем
        }
    }

    public bool CanUpgrade(ShipModule module)
    {
        try
        {
            var player = DI.Resolve<Player>();
            return player.TechPoints >= module.GetNextUpgradeCost();
        }
        catch
        {
            return false;
        }
    }

    public void UpgradeModule(ShipModule module)
    {
        if (!CanUpgrade(module)) return;

        try
        {
            var player = DI.Resolve<Player>();
            int cost = module.GetNextUpgradeCost();
            player.TechPoints -= cost;
            module.Level++;
            player.RecalculateShipStats();

            EventBus.Publish(new ModuleUpgradedEvent(module));
        }
        catch
        {
            // Игрок не найден — ничего не делаем
        }
    }
}

public class ModuleUpgradedEvent : GameEvent
{
    public ShipModule Module { get; }
    public ModuleUpgradedEvent(ShipModule module) => Module = module;
}