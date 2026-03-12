// Managers/SpaceCombatManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Models.Combat;
using ASTRANET.Models.Entities;
using ASTRANET.Utils;

namespace ASTRANET.Managers;

public class SpaceCombatManager
{
    public Player Player { get; private set; }
    public List<SpaceShip> Enemies { get; private set; } = new();
    public bool IsPlayerTurn { get; private set; } = true;
    public bool CombatActive { get; private set; } = true;
    public string Message { get; set; } = "";

    public int EscapeCounter { get; private set; } = -1;
    private WorldManager _worldManager;

    public SpaceCombatManager(Player player, List<SpaceShip> enemies, WorldManager worldManager)
    {
        Player = player;
        Enemies = enemies;
        _worldManager = worldManager;
    }

    public void StartCombat()
    {
        IsPlayerTurn = true;
        CombatActive = true;
        Message = "Бой начался!";
    }

    public void PlayerFire(SpaceWeapon weapon, SpaceShip target)
    {
        if (!IsPlayerTurn || !CombatActive) return;

        if (!PlayerCanFire(weapon))
        {
            Message = "Недостаточно энергии или оружие не готово";
            return;
        }

        Player.Energy -= weapon.EnergyCost;

        int hitChance = 80 + weapon.Accuracy + Player.Dexterity - target.Evasion;
        hitChance = Math.Clamp(hitChance, 5, 95);

        if (RandomManager.Next(100) >= hitChance)
        {
            Message = "Промах!";
            EndPlayerTurn();
            return;
        }

        int damage = RandomManager.Next(weapon.MinDamage, weapon.MaxDamage + 1);
        bool isCrit = RandomManager.NextDouble() < weapon.CritChance;
        if (isCrit)
        {
            damage = (int)(damage * weapon.CritMultiplier);
            Message = "Критическое попадание! ";
        }

        target.TakeDamage(damage, weapon.DamageType);

        Message += $"Нанесено {damage} урона {target.Name}.";

        if (!target.IsAlive)
        {
            Enemies.Remove(target);
            Message += $" {target.Name} уничтожен!";

            try
            {
                var techManager = DI.Resolve<TechManager>();
                techManager.AddTechPoints(5);
            }
            catch { }

            EventBus.Publish(new EnemyKilledEvent
            {
                EnemyId = target.Id,
                EnemyFaction = target.Faction,
                RewardCredits = 0
            });

            if (Enemies.Count == 0)
            {
                CombatActive = false;
                Message = "Победа!";
                return;
            }
        }

        EndPlayerTurn();
    }

    private bool PlayerCanFire(SpaceWeapon weapon)
    {
        return Player.Energy >= weapon.EnergyCost && weapon.IsReady;
    }

    private void EndPlayerTurn()
    {
        IsPlayerTurn = false;

        if (EscapeCounter > 0)
        {
            EscapeCounter--;
            Player.Energy = 0;
            if (EscapeCounter == 0)
            {
                PerformEscape();
                return;
            }
        }

        EnemyTurn();
    }

    private void EnemyTurn()
    {
        foreach (var enemy in Enemies)
        {
            enemy.BeginTurn();
            var weapon = enemy.Weapons.FirstOrDefault();
            if (weapon != null && enemy.Energy >= weapon.EnergyCost)
            {
                enemy.Energy -= weapon.EnergyCost;

                int hitChance = 80 + weapon.Accuracy + enemy.Accuracy - (Player.Dexterity / 2);
                hitChance = Math.Clamp(hitChance, 5, 95);

                if (RandomManager.Next(100) < hitChance)
                {
                    int damage = RandomManager.Next(weapon.MinDamage, weapon.MaxDamage + 1);
                    bool isCrit = RandomManager.NextDouble() < weapon.CritChance;
                    if (isCrit) damage = (int)(damage * weapon.CritMultiplier);

                    Player.TakeHullDamage(damage);

                    Message = $"Враг попал! Нанесено {damage} урона.";

                    if (Player.Hull <= 0)
                    {
                        CombatActive = false;
                        Message = "Поражение...";
                        return;
                    }
                }
                else
                {
                    Message = "Враг промахнулся.";
                }
            }
        }

        IsPlayerTurn = true;
        Player.Regenerate();
    }

    public void ActivateEscape()
    {
        if (EscapeCounter == -1)
        {
            EscapeCounter = RandomManager.Next(1, 4);
            Message = $"Активирован аварийный прыжок! Осталось ходов: {EscapeCounter}";
        }
    }

    private void PerformEscape()
    {
        CombatActive = false;
        Message = "Аварийный прыжок совершён!";

        var currentSystem = _worldManager.CurrentSystem;
        if (currentSystem != null && currentSystem.ConnectedSystems.Count > 0)
        {
            int randomIndex = RandomManager.Next(currentSystem.ConnectedSystems.Count);
            int targetSystemId = currentSystem.ConnectedSystems[randomIndex];
            _worldManager.TryEnterSystem(targetSystemId, 0);
        }
    }

    public void PlayerSkipTurn()
    {
        if (!IsPlayerTurn) return;
        EndPlayerTurn();
    }
}