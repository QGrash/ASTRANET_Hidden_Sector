using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Interior;
using ASTRANET_Hidden_Sector.Entities.Quest;
using ASTRANET_Hidden_Sector.World;
using ASTRANET_Hidden_Sector.Combat;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class InteriorMapScreen : GameScreen
    {
        private InteriorMap map;
        private string locationName;
        private const int MAP_OFFSET_X = 2;
        private const int MAP_OFFSET_Y = 3;
        private const int CELL_WIDTH = 2;

        private GroundCombatManager? combatManager;
        private bool inCombat = false;
        private List<NpcEntity> enemiesInCombat;
        private int selectedEnemyIndex = 0;

        public InteriorMapScreen(GameStateManager stateManager, UIManager uiManager, InteriorMap interiorMap, string name)
            : base(stateManager, uiManager)
        {
            map = interiorMap;
            locationName = name;
            enemiesInCombat = new List<NpcEntity>();
        }

        public override void Render()
        {
            uiManager.Clear();

            string title = $"Интерьер: {locationName}";
            for (int i = 0; i < title.Length; i++)
                uiManager.SetPixel(2 + i, 1, title[i], ConsoleColor.Cyan);

            DrawFrame();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var cell = map.GetCell(x, y);
                    if (cell == null) continue;

                    int screenX = MAP_OFFSET_X + x * CELL_WIDTH;
                    int screenY = MAP_OFFSET_Y + y;

                    char symbol;
                    ConsoleColor color;

                    if (x == map.PlayerX && y == map.PlayerY)
                    {
                        symbol = 'T';
                        color = ConsoleColor.White;
                    }
                    else if (cell.Entity != null && !cell.Entity.IsDestroyed)
                    {
                        symbol = cell.Entity.Symbol;
                        color = cell.Entity.Color;
                    }
                    else
                    {
                        symbol = cell.FloorSymbol;
                        color = cell.FloorColor;
                    }

                    uiManager.SetPixel(screenX, screenY, symbol, color);
                    uiManager.SetPixel(screenX + 1, screenY, ' ', ConsoleColor.Black);
                }
            }

            if (inCombat && combatManager != null)
            {
                int infoX = 50;
                int infoY = 3;

                string combatHeader = "=== БОЙ ===";
                for (int i = 0; i < combatHeader.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, combatHeader[i], ConsoleColor.Red);
                infoY++;

                string playerHealth = $"Игрок: {combatManager.GetPlayerHealth()}/{combatManager.GetPlayerMaxHealth()} HP";
                for (int i = 0; i < playerHealth.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, playerHealth[i], ConsoleColor.Green);
                infoY++;

                string playerEnergy = $"Энергия: {combatManager.GetPlayerEnergy()}/{combatManager.GetPlayerMaxEnergy()}";
                for (int i = 0; i < playerEnergy.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, playerEnergy[i], ConsoleColor.Yellow);
                infoY++;

                string enemiesHeader = "Враги:";
                for (int i = 0; i < enemiesHeader.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, enemiesHeader[i], ConsoleColor.Red);
                infoY++;

                for (int i = 0; i < enemiesInCombat.Count; i++)
                {
                    var enemy = enemiesInCombat[i];
                    if (enemy.CombatStats == null || !enemy.CombatStats.IsAlive) continue;

                    string line;
                    if (i == selectedEnemyIndex)
                        line = "> ";
                    else
                        line = "  ";
                    line += $"{enemy.Name} HP: {enemy.CombatStats.Health}/{enemy.CombatStats.MaxHealth}";

                    for (int j = 0; j < line.Length; j++)
                        uiManager.SetPixel(infoX + 2 + j, infoY, line[j],
                            i == selectedEnemyIndex ? ConsoleColor.Yellow : ConsoleColor.Red);
                    infoY++;
                }
            }
            else
            {
                var currentCell = map.GetCell(map.PlayerX, map.PlayerY);
                if (currentCell?.Entity != null && !currentCell.Entity.IsDestroyed)
                {
                    var entity = currentCell.Entity;
                    int infoX = 50;
                    int infoY = 3;

                    string obj = $"Объект: {entity.Name}";
                    for (int i = 0; i < obj.Length; i++)
                        uiManager.SetPixel(infoX + i, infoY, obj[i], ConsoleColor.Yellow);
                    infoY++;

                    string type = $"Тип: {entity.GetType().Name}";
                    for (int i = 0; i < type.Length; i++)
                        uiManager.SetPixel(infoX + i, infoY, type[i], ConsoleColor.Green);
                    infoY++;

                    for (int i = 0; i < entity.Description.Length; i++)
                        uiManager.SetPixel(infoX + i, infoY, entity.Description[i], ConsoleColor.Gray);
                }
            }

            string hint;
            if (inCombat)
                hint = "Стрелки - перемещение, Space - пропустить ход, Enter - атака выбранного врага, I - инвентарь, C - персонаж, B - корабль, J - квесты, Backspace - назад, ESC - меню";
            else
                hint = "Стрелки - перемещение, E - взаимодействие, I - инвентарь, C - персонаж, B - корабль, J - квесты, Backspace - назад, ESC - меню";

            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        private void DrawFrame()
        {
            int left = MAP_OFFSET_X - 1;
            int right = left + map.Width * CELL_WIDTH;
            int top = MAP_OFFSET_Y - 1;
            int bottom = top + map.Height + 1;

            for (int x = left + 1; x < right; x++)
            {
                uiManager.SetPixel(x, top, '─', ConsoleColor.Gray);
                uiManager.SetPixel(x, bottom, '─', ConsoleColor.Gray);
            }

            for (int y = top + 1; y < bottom; y++)
            {
                uiManager.SetPixel(left, y, '│', ConsoleColor.Gray);
                uiManager.SetPixel(right, y, '│', ConsoleColor.Gray);
            }

            uiManager.SetPixel(left, top, '┌', ConsoleColor.Gray);
            uiManager.SetPixel(right, top, '┐', ConsoleColor.Gray);
            uiManager.SetPixel(left, bottom, '└', ConsoleColor.Gray);
            uiManager.SetPixel(right, bottom, '┘', ConsoleColor.Gray);
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            int newX = map.PlayerX;
            int newY = map.PlayerY;

            if (inCombat && combatManager != null)
            {
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (enemiesInCombat.Count > 0)
                            selectedEnemyIndex = (selectedEnemyIndex - 1 + enemiesInCombat.Count) % enemiesInCombat.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        if (enemiesInCombat.Count > 0)
                            selectedEnemyIndex = (selectedEnemyIndex + 1) % enemiesInCombat.Count;
                        break;
                    case ConsoleKey.Enter:
                        AttackSelectedEnemy();
                        break;
                    case ConsoleKey.Spacebar:
                        SkipCombatTurn();
                        break;
                    default:
                        return;
                }
            }
            else
            {
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow: newY--; break;
                    case ConsoleKey.DownArrow: newY++; break;
                    case ConsoleKey.LeftArrow: newX--; break;
                    case ConsoleKey.RightArrow: newX++; break;
                    case ConsoleKey.E:
                        InteractWithCurrentCell();
                        return;
                    case ConsoleKey.Backspace:
                        stateManager.PopScreen();
                        return;
                    default:
                        return;
                }

                if (newX >= 0 && newX < map.Width && newY >= 0 && newY < map.Height)
                {
                    var targetCell = map.GetCell(newX, newY);
                    if (targetCell != null)
                    {
                        if (targetCell.Entity != null && targetCell.Entity.IsSolid)
                            return;
                        map.PlayerX = newX;
                        map.PlayerY = newY;
                    }
                }
            }
        }

        private void InteractWithCurrentCell()
        {
            var cell = map.GetCell(map.PlayerX, map.PlayerY);
            if (cell?.Entity != null && !cell.Entity.IsDestroyed)
            {
                if (cell.Entity is NpcEntity npc && npc.IsHostile)
                {
                    StartCombat(new List<NpcEntity> { npc });
                }
                else
                {
                    QuestManager.UpdateProgress(ObjectiveType.VisitLocation, cell.Entity.Name, 1);
                    cell.Entity.Interact(stateManager, uiManager);
                    if (cell.Entity.IsDestroyed)
                    {
                        cell.Entity = null;
                    }
                }
            }
            else
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Пусто", "Здесь ничего нет."));
            }
        }

        private void StartCombat(List<NpcEntity> enemies)
        {
            var playerWeapon = new Weapon("Лазерный пистолет", DamageType.Laser, 5, 8, 5, 5, 1, 5, 1.5f, WeaponClass.Light, WeaponMode.Single);
            var playerCombatant = new GroundCombatant("Игрок", 100, 50, 10, 10, playerWeapon);

            enemiesInCombat = enemies;
            combatManager = new GroundCombatManager(playerCombatant, enemiesInCombat, map.PlayerX, map.PlayerY);
            inCombat = true;
            selectedEnemyIndex = 0;
        }

        private void AttackSelectedEnemy()
        {
            if (selectedEnemyIndex < 0 || selectedEnemyIndex >= enemiesInCombat.Count) return;
            var target = enemiesInCombat[selectedEnemyIndex];
            combatManager?.PlayerAttack(target);
            CheckCombatEnd();
        }

        private void SkipCombatTurn()
        {
            combatManager?.PlayerSkipTurn();
            CheckCombatEnd();
        }

        private void CheckCombatEnd()
        {
            if (combatManager == null) return;

            if (!combatManager.IsPlayerAlive())
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Поражение",
                    "Вы погибли... Нажмите любую клавишу для выхода."));
                stateManager.ChangeScreen(new MainMenuScreen(stateManager, uiManager));
            }
            else if (!combatManager.IsInCombat)
            {
                inCombat = false;
                foreach (var enemy in enemiesInCombat)
                {
                    var cell = map.GetCell(enemy.X, enemy.Y);
                    if (cell != null) cell.Entity = null;
                }
                enemiesInCombat.Clear();
                combatManager = null;
            }
        }

        public InteriorMap GetMap() => map;
        public int GetPlayerX() => map.PlayerX;
        public int GetPlayerY() => map.PlayerY;
        public string GetLocationName() => locationName;
    }
}