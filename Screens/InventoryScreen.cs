// Screens/InventoryScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Instances;

namespace ASTRANET.Screens;

public class InventoryScreen : BaseScreen
{
    private readonly Player _player;
    private List<ItemInstance> _items;
    private int _selectedIndex = 0;
    private int _scrollOffset = 0;
    private const int ItemsPerPage = 10;
    private readonly List<(string text, ConsoleColor color)> _hints;

    public InventoryScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager)
        : base(uiManager, inputManager, screenManager)
    {
        _player = DI.Resolve<Player>();
        RefreshItems();

        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓ - выбор", ConsoleColor.Gray),
            ("Enter - использовать/экипировать/установить модуль", ConsoleColor.Gray),
            ("D - выбросить", ConsoleColor.Gray),
            ("Backspace - назад", ConsoleColor.Gray)
        };
    }

    private void RefreshItems()
    {
        _items = _player.Inventory.Items.ToList();
    }

    public override void Render()
    {
        UIManager.Clear();

        UIManager.DrawString(2, 2, "ИНВЕНТАРЬ", ConsoleColor.Yellow);
        UIManager.DrawString(2, 3, new string('=', 30), ConsoleColor.DarkGray);

        double totalWeight = _player.GetTotalWeight();
        int maxWeight = _player.GetMaxWeight();
        string weightStr = $"Вес: {totalWeight:F1}/{maxWeight} кг";
        ConsoleColor weightColor = totalWeight > maxWeight ? ConsoleColor.Red : ConsoleColor.Gray;
        UIManager.DrawString(2, 4, weightStr, weightColor);

        UIManager.DrawString(2, 5, $"TechPoints: {_player.TechPoints}", ConsoleColor.Cyan);

        int startY = 7;
        for (int i = 0; i < ItemsPerPage; i++)
        {
            int idx = _scrollOffset + i;
            if (idx >= _items.Count) break;

            var item = _items[idx];
            string line = item.Quantity > 1
                ? $"{item.Prototype?.Name} x{item.Quantity}"
                : item.Prototype?.Name ?? "Неизвестный предмет";

            bool isInstalled = _player.InstalledModules.Any(m => m.Item == item);

            if (idx == _selectedIndex)
                UIManager.DrawString(2, startY + i, "> " + line + (isInstalled ? " (установлен)" : ""), ConsoleColor.Yellow);
            else
                UIManager.DrawString(2, startY + i, "  " + line + (isInstalled ? " (установлен)" : ""), ConsoleColor.White);
        }

        UIManager.DrawHints(_hints);
    }

    public override bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (_selectedIndex > 0)
                {
                    _selectedIndex--;
                    if (_selectedIndex < _scrollOffset)
                        _scrollOffset = _selectedIndex;
                }
                return true;
            case ConsoleKey.DownArrow:
                if (_selectedIndex < _items.Count - 1)
                {
                    _selectedIndex++;
                    if (_selectedIndex >= _scrollOffset + ItemsPerPage)
                        _scrollOffset = _selectedIndex - ItemsPerPage + 1;
                }
                return true;
            case ConsoleKey.Enter:
                UseSelectedItem();
                RefreshItems();
                return true;
            case ConsoleKey.D:
                DropSelectedItem();
                RefreshItems();
                return true;
            case ConsoleKey.Backspace:
                ScreenManager.PopScreen();
                return true;
            default:
                return false;
        }
    }

    private void UseSelectedItem()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _items.Count) return;
        var item = _items[_selectedIndex];
        if (item.Prototype == null) return;

        bool isInstalled = _player.InstalledModules.Any(m => m.Item == item);

        if (item.Prototype.ItemType == ItemType.Consumable)
        {
            if (item.Prototype.Effects != null && item.Prototype.Effects.ContainsKey("heal"))
            {
                _player.Heal(item.Prototype.Effects["heal"]);
            }
            _player.Inventory.RemoveItem(item, 1);
        }
        else if (item.Prototype.ItemType == ItemType.Equipment)
        {
            if (item.Prototype.Slot.HasValue)
            {
                ItemInstance old = null;
                switch (item.Prototype.Slot.Value)
                {
                    case EquipmentSlot.Head: old = _player.Head; _player.Head = item; break;
                    case EquipmentSlot.Torso: old = _player.Torso; _player.Torso = item; break;
                    case EquipmentSlot.Body: old = _player.Body; _player.Body = item; break;
                    case EquipmentSlot.Hands: old = _player.Hands; _player.Hands = item; break;
                    case EquipmentSlot.Legs: old = _player.Legs; _player.Legs = item; break;
                    case EquipmentSlot.Weapon: old = _player.Weapon; _player.Weapon = item; break;
                    case EquipmentSlot.Neck: old = _player.Neck; _player.Neck = item; break;
                    case EquipmentSlot.Belt:
                        if (_player.Belt.Count < 3)
                            _player.Belt.Add(item);
                        else
                            return;
                        break;
                }
                if (old != null)
                    _player.Inventory.AddItem(old);
                _player.Inventory.RemoveItem(item);
            }
        }
        else if (item.Prototype.ItemType == ItemType.Module)
        {
            if (isInstalled)
            {
                var module = _player.InstalledModules.First(m => m.Item == item);
                _player.InstalledModules.Remove(module);
                UIManager.ShowMessage($"Модуль {item.Prototype.Name} снят");
            }
            else
            {
                var module = new ShipModule { Item = item, Level = 1 };
                _player.InstalledModules.Add(module);
                UIManager.ShowMessage($"Модуль {item.Prototype.Name} установлен");
            }
            _player.RecalculateShipStats();
        }
    }

    private void DropSelectedItem()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _items.Count) return;
        var item = _items[_selectedIndex];

        var module = _player.InstalledModules.FirstOrDefault(m => m.Item == item);
        if (module != null)
        {
            _player.InstalledModules.Remove(module);
        }

        _player.Inventory.RemoveItem(item);
        UIManager.ShowMessage($"Предмет {item.Prototype?.Name} выброшен");
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}