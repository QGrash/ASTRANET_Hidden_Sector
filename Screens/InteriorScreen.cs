// Screens/InteriorScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Instances;
using ASTRANET.Models.World;
using ASTRANET.Utils;

namespace ASTRANET.Screens;

public class InteriorScreen : BaseScreen
{
    private readonly Player _player;
    private readonly InteriorMap _map;
    private int _cameraX, _cameraY;
    private const int ViewWidth = 40;
    private const int ViewHeight = 20;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public InteriorScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager, InteriorMap map, Player player)
        : base(uiManager, inputManager, screenManager)
    {
        _player = player;
        _map = map;

        if (map.Exits.Count > 0)
        {
            var (ex, ey) = map.Exits[0];
            _player.X = ex;
            _player.Y = ey;
        }
        else
        {
            _player.X = map.Width / 2;
            _player.Y = map.Height / 2;
        }

        _cameraX = _player.X - ViewWidth / 2;
        _cameraY = _player.Y - ViewHeight / 2;
        ClampCamera();

        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓/←/→ - движение", ConsoleColor.Gray),
            ("T - взаимодействие/атака/обыск", ConsoleColor.Gray),
            ("I - инвентарь", ConsoleColor.Gray),
            ("Space - пропустить ход", ConsoleColor.Gray),
            ("Backspace - выйти", ConsoleColor.Gray)
        };
    }

    private void ClampCamera()
    {
        if (_cameraX < 0) _cameraX = 0;
        if (_cameraY < 0) _cameraY = 0;
        if (_cameraX > _map.Width - ViewWidth) _cameraX = _map.Width - ViewWidth;
        if (_cameraY > _map.Height - ViewHeight) _cameraY = _map.Height - ViewHeight;
    }

    public override void Render()
    {
        UIManager.Clear();

        for (int y = 0; y < ViewHeight; y++)
        {
            for (int x = 0; x < ViewWidth; x++)
            {
                int mapX = _cameraX + x;
                int mapY = _cameraY + y;
                if (mapX >= 0 && mapX < _map.Width && mapY >= 0 && mapY < _map.Height)
                {
                    var cell = _map.GetCell(mapX, mapY);
                    if (cell != null)
                    {
                        UIManager.SetPixel(x, y, cell.Tile, cell.Color);
                    }
                }
            }
        }

        foreach (var npc in _map.Npcs.Where(n => n.IsAlive))
        {
            int screenX = npc.X - _cameraX;
            int screenY = npc.Y - _cameraY;
            if (screenX >= 0 && screenX < ViewWidth && screenY >= 0 && screenY < ViewHeight)
            {
                char symbol = 'Y';
                ConsoleColor color;
                if (npc.Type == ASTRANET.NpcType.Enemy)
                    color = ConsoleColor.Red;
                else if (npc.Type == ASTRANET.NpcType.Neutral)
                    color = ConsoleColor.Yellow;
                else
                    color = ConsoleColor.Green;

                UIManager.SetPixel(screenX, screenY, symbol, color);
            }
        }

        foreach (var npc in _map.Npcs.Where(n => n.IsDead))
        {
            int screenX = npc.X - _cameraX;
            int screenY = npc.Y - _cameraY;
            if (screenX >= 0 && screenX < ViewWidth && screenY >= 0 && screenY < ViewHeight)
            {
                UIManager.SetPixel(screenX, screenY, 'X', ConsoleColor.DarkGray);
            }
        }

        int playerScreenX = _player.X - _cameraX;
        int playerScreenY = _player.Y - _cameraY;
        if (playerScreenX >= 0 && playerScreenX < ViewWidth && playerScreenY >= 0 && playerScreenY < ViewHeight)
        {
            UIManager.SetPixel(playerScreenX, playerScreenY, 'Y', ConsoleColor.White);
        }

        UIManager.DrawHints(_hints);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        bool actionPerformed = false;

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.DownArrow:
            case ConsoleKey.LeftArrow:
            case ConsoleKey.RightArrow:
                actionPerformed = TryMove(keyInfo.Key);
                break;
            case ConsoleKey.T:
                actionPerformed = HandleInteraction();
                break;
            case ConsoleKey.I:
                // Инвентарь — не тратит ход
                return true;
            case ConsoleKey.Spacebar:
                SkipTurn();
                actionPerformed = true;
                break;
            case ConsoleKey.Backspace:
                ScreenManager.PopScreen();
                return true;
            default:
                return false;
        }

        if (actionPerformed)
        {
            ProcessNPCTurn();
        }
        return true;
    }

    private bool TryMove(ConsoleKey key)
    {
        int newX = _player.X, newY = _player.Y;
        switch (key)
        {
            case ConsoleKey.UpArrow: newY--; break;
            case ConsoleKey.DownArrow: newY++; break;
            case ConsoleKey.LeftArrow: newX--; break;
            case ConsoleKey.RightArrow: newX++; break;
        }

        if (newX < 0 || newX >= _map.Width || newY < 0 || newY >= _map.Height)
            return false;

        var cell = _map.GetCell(newX, newY);
        if (cell != null && cell.Walkable)
        {
            // Проверяем, нет ли там живого NPC
            if (_map.Npcs.Any(n => n.X == newX && n.Y == newY && n.IsAlive))
            {
                UIManager.ShowMessage("Путь преграждён.");
                return false;
            }

            _player.X = newX;
            _player.Y = newY;
            _cameraX = _player.X - ViewWidth / 2;
            _cameraY = _player.Y - ViewHeight / 2;
            ClampCamera();
            return true;
        }
        return false;
    }

    private bool HandleInteraction()
    {
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };
        List<InteriorNpc> adjacentNpcs = new();

        for (int i = 0; i < 4; i++)
        {
            int nx = _player.X + dx[i];
            int ny = _player.Y + dy[i];
            var npc = _map.Npcs.FirstOrDefault(n => n.X == nx && n.Y == ny);
            if (npc != null)
                adjacentNpcs.Add(npc);
        }

        if (adjacentNpcs.Count == 0)
        {
            // Проверяем контейнеры
            for (int i = 0; i < 4; i++)
            {
                int nx = _player.X + dx[i];
                int ny = _player.Y + dy[i];
                if (_map.Containers.Contains((nx, ny)))
                {
                    OpenContainer(nx, ny);
                    return true; // действие выполнено (ход потрачен)
                }
            }

            if (_map.Exits.Contains((_player.X, _player.Y)))
            {
                UIManager.ShowMessage("Вы вышли наружу.");
                ScreenManager.PopScreen();
                return false; // выход не тратит ход? лучше не тратить
            }
            return false;
        }

        var target = adjacentNpcs[0];
        if (target.IsAlive)
        {
            if (target.Type == ASTRANET.NpcType.Enemy)
            {
                AttackNpc(target);
                return true; // атака тратит ход
            }
            else
            {
                StartDialogueWithNpc(target);
                return false; // диалог не тратит ход (можно и тратить, но обычно нет)
            }
        }
        else
        {
            LootCorpse(target);
            return true; // обыск трупа тратит ход? можно и да
        }
    }

    private void AttackNpc(InteriorNpc target)
    {
        int damage = _player.Strength + RandomManager.Next(1, 6);
        damage -= target.Defense;
        if (damage < 1) damage = 1;

        target.Health -= damage;
        UIManager.ShowMessage($"Вы нанесли {damage} урона {target.Name}.");

        if (!target.IsAlive)
        {
            UIManager.ShowMessage($"{target.Name} убит! Теперь можно обыскать труп.");
        }
        else
        {
            int enemyDamage = target.Attack - _player.GetTotalDefense();
            if (enemyDamage < 1) enemyDamage = 1;
            _player.TakeDamage(enemyDamage);
            UIManager.ShowMessage($"{target.Name} ответил, нанеся {enemyDamage} урона.");
        }
    }

    private void StartDialogueWithNpc(InteriorNpc target)
    {
        if (string.IsNullOrEmpty(target.DialogueId))
        {
            UIManager.ShowMessage($"{target.Name} не хочет разговаривать.");
            return;
        }

        var dialogueManager = DI.Resolve<DialogueManager>();
        if (dialogueManager.StartDialogue(target.DialogueId))
        {
            var dialogueScreen = new DialogueScreen(UIManager, ScreenManager, InputManager, target.Name);
            ScreenManager.PushScreen(dialogueScreen);
        }
        else
        {
            UIManager.ShowMessage($"Ошибка: диалог {target.DialogueId} не найден!");
        }
    }

    private void LootCorpse(InteriorNpc corpse)
    {
        if (corpse.Inventory.Count == 0)
        {
            UIManager.ShowMessage("Ничего нет.");
            _map.Npcs.Remove(corpse);
            return;
        }

        foreach (var item in corpse.Inventory)
        {
            _player.Inventory.AddItem(item);
            UIManager.ShowMessage($"Получено: {item.Prototype?.Name}");
        }
        _map.Npcs.Remove(corpse);
    }

    private void OpenContainer(int x, int y)
    {
        var itemManager = DI.Resolve<ItemManager>();
        var possibleItems = new[] { "medkit_basic", "energy_cell", "metal_alloy" };
        string itemId = possibleItems[RandomManager.Next(possibleItems.Length)];
        var proto = itemManager.GetPrototype(itemId);
        if (proto != null)
        {
            var item = new ItemInstance(proto.Id, 1) { Prototype = proto };
            _player.Inventory.AddItem(item);
            UIManager.ShowMessage($"Вы нашли: {proto.Name}");
        }

        _map.Containers.Remove((x, y));
        _map.SetCell(x, y, new InteriorCell('.', true, ConsoleColor.White));
    }

    private void SkipTurn()
    {
        _player.Regenerate();
        UIManager.ShowMessage("Ход пропущен. Энергия и щиты восстановлены.");
    }

    private void ProcessNPCTurn()
    {
        // Сначала обрабатываем врагов
        foreach (var npc in _map.Npcs.Where(n => n.IsAlive && n.Type == ASTRANET.NpcType.Enemy))
        {
            // Если враг рядом с игроком — атакует
            if (Math.Abs(npc.X - _player.X) <= 1 && Math.Abs(npc.Y - _player.Y) <= 1)
            {
                int damage = npc.Attack - _player.GetTotalDefense();
                if (damage < 1) damage = 1;
                _player.TakeDamage(damage);
                UIManager.ShowMessage($"{npc.Name} атакует вас, нанося {damage} урона.");
                continue;
            }

            // Иначе пытаемся двигаться к игроку (простой ИИ: шаг по оси X или Y)
            // Определяем направление
            int dx = 0, dy = 0;
            if (npc.X < _player.X) dx = 1;
            else if (npc.X > _player.X) dx = -1;
            else if (npc.Y < _player.Y) dy = 1;
            else if (npc.Y > _player.Y) dy = -1;

            int newX = npc.X + dx;
            int newY = npc.Y + dy;

            // Проверяем, можно ли туда встать
            if (CanNPCStep(newX, newY))
            {
                npc.X = newX;
                npc.Y = newY;
            }
        }

        // Теперь нейтралы (патрулирующие)
        foreach (var npc in _map.Npcs.Where(n => n.IsAlive && n.Type == ASTRANET.NpcType.Neutral && n.Behavior == NpcBehavior.Patrol))
        {
            // Двигаемся к цели патруля
            int dx = 0, dy = 0;
            if (npc.X < npc.PatrolTargetX) dx = 1;
            else if (npc.X > npc.PatrolTargetX) dx = -1;
            else if (npc.Y < npc.PatrolTargetY) dy = 1;
            else if (npc.Y > npc.PatrolTargetY) dy = -1;

            int newX = npc.X + dx;
            int newY = npc.Y + dy;

            // Если достигли цели, выбираем новую
            if (newX == npc.PatrolTargetX && newY == npc.PatrolTargetY)
            {
                npc.PatrolTargetX = npc.X + RandomManager.Next(-3, 4);
                npc.PatrolTargetY = npc.Y + RandomManager.Next(-3, 4);
                // Принудительно зажимаем в пределах карты
                npc.PatrolTargetX = Math.Clamp(npc.PatrolTargetX, 0, _map.Width - 1);
                npc.PatrolTargetY = Math.Clamp(npc.PatrolTargetY, 0, _map.Height - 1);
            }

            if (CanNPCStep(newX, newY))
            {
                npc.X = newX;
                npc.Y = newY;
            }
        }
    }

    private bool CanNPCStep(int x, int y)
    {
        // Проверка границ
        if (x < 0 || x >= _map.Width || y < 0 || y >= _map.Height)
            return false;

        var cell = _map.GetCell(x, y);
        if (cell == null || !cell.Walkable)
            return false;

        // Нельзя вставать на клетку с другим живым NPC
        if (_map.Npcs.Any(n => n.X == x && n.Y == y && n.IsAlive))
            return false;

        // Нельзя вставать на игрока (враг может атаковать, но это обрабатывается отдельно)
        if (_player.X == x && _player.Y == y)
            return false;

        return true;
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}