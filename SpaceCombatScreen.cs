using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Combat;
using ASTRANET_Hidden_Sector.World;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class SpaceCombatScreen : GameScreen
    {
        private SpaceCombatant playerShip;
        private List<SpaceCombatant> enemies;
        private StarSystem currentSystem;
        private int selectedEnemyIndex = 0;
        private int selectedWeaponIndex = 0;
        private List<string> logMessages = new List<string>();
        private bool playerTurn = true;

        private const int ENERGY_GAIN_ON_SKIP = 15;
        private const int EVASION_BONUS_ON_SKIP = 5;

        private bool isEscaping = false;
        private int escapeTurnsLeft = 0;
        private int escapeTurnsTotal = 0;

        public SpaceCombatScreen(GameStateManager stateManager, UIManager uiManager, SpaceCombatant player, List<SpaceCombatant> enemies, StarSystem system)
            : base(stateManager, uiManager)
        {
            playerShip = player;
            this.enemies = enemies;
            currentSystem = system;
            playerShip.BeginTurn();
            logMessages.Add("Бой начался!");
        }

        private void EnemyTurn()
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;
                enemy.BeginTurn();
                if (enemy.Weapons.Count > 0)
                {
                    var weapon = enemy.Weapons[0];
                    if (enemy.CanFire(weapon))
                    {
                        int dmg = enemy.FireWeapon(weapon, playerShip);
                        logMessages.Add($"{enemy.Name} атакует и наносит {dmg} урона.");
                    }
                }
            }
            playerTurn = true;
            PlayerTurnStart();
            CheckVictory();
        }

        private void PlayerTurnStart()
        {
            playerShip.BeginTurn();
            if (isEscaping)
            {
                escapeTurnsLeft--;
                if (escapeTurnsLeft <= 0)
                {
                    EscapeSuccess();
                }
                else
                {
                    playerShip.Energy = 0;
                    logMessages.Add($"Прыжок: осталось {escapeTurnsLeft} ход(ов). Энергия обнулена.");
                }
            }
        }

        private void CheckVictory()
        {
            if (!playerShip.IsAlive)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Поражение",
                    "Ваш корабль уничтожен. Нажмите любую клавишу для выхода в главное меню."));
                stateManager.ChangeScreen(new MainMenuScreen(stateManager, uiManager));
            }
            else if (enemies.TrueForAll(e => !e.IsAlive))
            {
                logMessages.Add("Все враги уничтожены! Победа!");
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Победа",
                    "Вы победили! Нажмите любую клавишу, чтобы вернуться на карту."));
                stateManager.PopScreen();
            }
        }

        public override void Render()
        {
            uiManager.Clear();

            int leftX = 5;
            int topY = 3;

            string header = "=== КОСМИЧЕСКИЙ БОЙ ===";
            for (int i = 0; i < header.Length; i++)
                uiManager.SetPixel(leftX + i, topY - 1, header[i], ConsoleColor.Red);

            // Информация об игроке
            string name = $"Игрок: {playerShip.Name}";
            for (int i = 0; i < name.Length; i++)
                uiManager.SetPixel(leftX + i, topY, name[i], ConsoleColor.Green);
            topY++;

            string hull = $"Корпус: {playerShip.Health}/{playerShip.MaxHealth}";
            for (int i = 0; i < hull.Length; i++)
                uiManager.SetPixel(leftX + i, topY, hull[i], ConsoleColor.White);
            topY++;

            if (playerShip.Shield != null)
            {
                string shield = $"Щиты: {playerShip.Shield.Current}/{playerShip.Shield.Max}";
                for (int i = 0; i < shield.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, shield[i], ConsoleColor.Cyan);
                topY++;
            }

            string energy = $"Энергия: {playerShip.Energy}/{playerShip.MaxEnergy}";
            for (int i = 0; i < energy.Length; i++)
                uiManager.SetPixel(leftX + i, topY, energy[i], ConsoleColor.Yellow);
            topY++;

            if (playerShip.EvasionBonus > 0)
            {
                string bonus = $"+{playerShip.EvasionBonus}% укл.";
                for (int i = 0; i < bonus.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, bonus[i], ConsoleColor.Magenta);
                topY++;
            }

            if (isEscaping)
            {
                string escape = $"Побег: {escapeTurnsLeft}/{escapeTurnsTotal}";
                for (int i = 0; i < escape.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, escape[i], ConsoleColor.DarkYellow);
                topY++;
            }

            topY += 2;
            string enemiesHeader = "Враги:";
            for (int i = 0; i < enemiesHeader.Length; i++)
                uiManager.SetPixel(leftX + i, topY, enemiesHeader[i], ConsoleColor.Red);
            topY++;

            for (int i = 0; i < enemies.Count; i++)
            {
                var e = enemies[i];
                string status = e.IsAlive ? $"{e.Health}/{e.MaxHealth}" : "Уничтожен";
                string line;
                if (i == selectedEnemyIndex)
                    line = "> ";
                else
                    line = "  ";
                line += $"{e.Name} [HP: {status}]";

                ConsoleColor color = e.IsAlive ? ConsoleColor.Red : ConsoleColor.DarkGray;
                for (int j = 0; j < line.Length; j++)
                    uiManager.SetPixel(leftX + j, topY, line[j], color);
                topY++;
            }

            topY += 2;
            string logHeader = "Лог:";
            for (int i = 0; i < logHeader.Length; i++)
                uiManager.SetPixel(leftX + i, topY, logHeader[i], ConsoleColor.Cyan);
            topY++;

            int startLogY = topY;
            for (int i = Math.Max(0, logMessages.Count - 5); i < logMessages.Count; i++)
            {
                string msg = logMessages[i];
                for (int j = 0; j < msg.Length; j++)
                    uiManager.SetPixel(leftX + j, startLogY + (i - Math.Max(0, logMessages.Count - 5)), msg[j], ConsoleColor.Gray);
            }

            topY = startLogY + 6;
            string weaponsHeader = "Ваше оружие:";
            for (int i = 0; i < weaponsHeader.Length; i++)
                uiManager.SetPixel(leftX + i, topY, weaponsHeader[i], ConsoleColor.Yellow);
            topY++;

            for (int i = 0; i < playerShip.Weapons.Count; i++)
            {
                var w = playerShip.Weapons[i];
                string canFire = playerShip.CanFire(w) ? "" : "(недостаточно энергии)";
                string line;
                if (i == selectedWeaponIndex)
                    line = "> ";
                else
                    line = "  ";
                line += $"{w.Name} [{w.MinDamage}-{w.MaxDamage}] {canFire}";

                for (int j = 0; j < line.Length; j++)
                    uiManager.SetPixel(leftX + j, topY, line[j],
                        playerShip.CanFire(w) ? ConsoleColor.White : ConsoleColor.DarkGray);
                topY++;
            }

            string hint1;
            if (!isEscaping)
                hint1 = "Space - пропустить ход, F - аварийный прыжок (1-3 хода)";
            else
                hint1 = "Побег... Вы не можете атаковать.";
            for (int i = 0; i < hint1.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 3, hint1[i], ConsoleColor.DarkGray);

            string hint2 = "Enter - атака (если есть энергия), Tab - выбор оружия, ↑/↓ - выбор цели, I - инвентарь, C - персонаж, B - корабль, J - квесты, ESC - меню";
            for (int i = 0; i < hint2.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint2[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            if (!playerTurn) return;

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedEnemyIndex = (selectedEnemyIndex - 1 + enemies.Count) % enemies.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedEnemyIndex = (selectedEnemyIndex + 1) % enemies.Count;
                    break;
                case ConsoleKey.Tab:
                    selectedWeaponIndex = (selectedWeaponIndex + 1) % playerShip.Weapons.Count;
                    break;
                case ConsoleKey.Enter:
                    if (!isEscaping)
                        AttackSelectedEnemy();
                    else
                        logMessages.Add("Вы не можете атаковать во время прыжка!");
                    break;
                case ConsoleKey.Spacebar:
                    SkipTurn();
                    break;
                case ConsoleKey.F:
                    if (!isEscaping)
                        TryEscape();
                    else
                        logMessages.Add("Прыжок уже запущен!");
                    break;
            }
        }

        private void AttackSelectedEnemy()
        {
            if (selectedEnemyIndex >= enemies.Count || !enemies[selectedEnemyIndex].IsAlive) return;
            var weapon = playerShip.Weapons[selectedWeaponIndex];
            if (!playerShip.CanFire(weapon)) return;

            var target = enemies[selectedEnemyIndex];
            int damage = playerShip.FireWeapon(weapon, target);
            logMessages.Add($"Вы атаковали {target.Name} и нанесли {damage} урона.");

            playerTurn = false;
            EnemyTurn();
        }

        private void SkipTurn()
        {
            playerShip.GainEnergyOnSkip(ENERGY_GAIN_ON_SKIP);
            playerShip.EvasionBonus += EVASION_BONUS_ON_SKIP;
            logMessages.Add($"Вы пропустили ход: восстановлено {ENERGY_GAIN_ON_SKIP} энергии, +{EVASION_BONUS_ON_SKIP}% уклонения.");

            playerTurn = false;
            EnemyTurn();
        }

        private void TryEscape()
        {
            Random rand = new Random();
            escapeTurnsTotal = rand.Next(1, 4);
            escapeTurnsLeft = escapeTurnsTotal;
            isEscaping = true;
            logMessages.Add($"Аварийный прыжок запущен! Осталось ходов: {escapeTurnsLeft}. Энергия обнуляется!");

            playerShip.Energy = 0;
            playerTurn = false;
            EnemyTurn();
        }

        private void EscapeSuccess()
        {
            logMessages.Add("Прыжок удался! Вы покидаете бой.");
            var connected = currentSystem.ConnectedSystems;
            if (connected.Count == 0)
            {
                logMessages.Add("Нет доступных систем для побега! Возврат на карту.");
                stateManager.PopScreen(); // просто закрываем бой
                return;
            }

            Random rand = new Random();
            var targetSystem = connected[rand.Next(connected.Count)];

            string fromName = currentSystem.Name;
            string toName = targetSystem.Name;

            Action onJumpComplete = () =>
            {
                Game.CurrentSystem = targetSystem;
                // Заменяем экран боя на карту сектора в новой системе
                stateManager.ReplaceScreen(new GlobalMapScreen(stateManager, uiManager, Game.CurrentSector, targetSystem));
            };

            stateManager.PushScreen(new HyperJumpScreen(stateManager, uiManager, fromName, toName, onJumpComplete));
            isEscaping = false;
        }

        public override void Update(float deltaTime)
        {
            // Не используется
        }
    }
}