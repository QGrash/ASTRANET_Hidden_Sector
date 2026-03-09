using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Entities.Interior;

namespace ASTRANET_Hidden_Sector.Combat
{
    public class GroundCombatManager
    {
        private List<NpcEntity> enemies;
        private GroundCombatant playerCombatant;
        private int playerX, playerY;
        private bool playerTurn = true;
        private const int ENERGY_GAIN_ON_SKIP = 10;
        private const int EVASION_BONUS_ON_SKIP = 5;

        public bool IsInCombat => enemies.Any(e => e.CombatStats != null && e.CombatStats.IsAlive);

        public GroundCombatManager(GroundCombatant player, List<NpcEntity> enemiesInCombat, int startX, int startY)
        {
            playerCombatant = player;
            enemies = enemiesInCombat.Where(e => e.IsHostile && e.CombatStats != null).ToList();
            playerX = startX;
            playerY = startY;
        }

        // Методы для доступа к состоянию игрока
        public int GetPlayerHealth() => playerCombatant.Health;
        public int GetPlayerMaxHealth() => playerCombatant.MaxHealth;
        public int GetPlayerEnergy() => playerCombatant.Energy;
        public int GetPlayerMaxEnergy() => playerCombatant.MaxEnergy;
        public bool IsPlayerAlive() => playerCombatant.IsAlive;

        public void UpdatePlayerPosition(int x, int y)
        {
            playerX = x;
            playerY = y;
        }

        public void PlayerTurn()
        {
            playerTurn = true;
            playerCombatant.BeginTurn();
        }

        public void PlayerSkipTurn()
        {
            if (!playerTurn) return;
            playerCombatant.GainEnergyOnSkip(ENERGY_GAIN_ON_SKIP);
            playerCombatant.EvasionBonus += EVASION_BONUS_ON_SKIP;
            playerTurn = false;
            EnemyTurn();
        }

        public void PlayerAttack(NpcEntity target)
        {
            if (!playerTurn || target?.CombatStats == null || !target.CombatStats.IsAlive) return;
            if (!playerCombatant.CanFire()) return;

            int damage = playerCombatant.FireWeapon(target.CombatStats);
            // Логирование можно добавить позже
            playerTurn = false;
            EnemyTurn();
        }

        private void EnemyTurn()
        {
            foreach (var enemy in enemies.Where(e => e.CombatStats != null && e.CombatStats.IsAlive))
            {
                enemy.CombatStats.BeginTurn();
                if (IsAdjacentToPlayer(enemy))
                {
                    if (enemy.CombatStats.CanFire())
                    {
                        int dmg = enemy.CombatStats.FireWeapon(playerCombatant);
                        // Логирование
                    }
                }
                else
                {
                    // Двигаться к игроку (будет реализовано позже)
                }
            }
            playerTurn = true;
            CheckVictory();
        }

        private bool IsAdjacentToPlayer(NpcEntity enemy)
        {
            int dx = System.Math.Abs(enemy.X - playerX);
            int dy = System.Math.Abs(enemy.Y - playerY);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        private void CheckVictory()
        {
            if (!playerCombatant.IsAlive)
            {
                // Поражение – можно сигнализировать
            }
            else if (enemies.All(e => e.CombatStats == null || !e.CombatStats.IsAlive))
            {
                // Победа
            }
        }
    }
}