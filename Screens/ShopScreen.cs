// Screens/ShopScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Managers;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Instances;
using ASTRANET.Models.Prototypes;

namespace ASTRANET.Screens;

public class ShopScreen : BaseScreen
{
    private readonly Player _player;
    private readonly ShopPrototype _shop;
    private readonly List<ShopItemInstance> _shopItems;
    private readonly List<(string text, ConsoleColor color)> _hints;
    private int _selectedIndex = 0;
    private int _scrollOffset = 0;
    private const int ItemsPerPage = 10;
    private bool _buyingMode = true;
    private int _selectedQuantity = 1;

    private class ShopItemInstance
    {
        public ItemPrototype Prototype { get; set; }
        public double PriceModifier { get; set; }
        public int AvailableQuantity { get; set; }
    }

    public ShopScreen(UIManager uiManager, ScreenManager screenManager, InputManager inputManager, ShopPrototype shop, Player player)
        : base(uiManager, inputManager, screenManager)
    {
        _player = player;
        _shop = shop;
        _shopItems = new List<ShopItemInstance>();
        var itemManager = DI.Resolve<ItemManager>();
        var reputationManager = DI.Resolve<ReputationManager>();

        foreach (var item in shop.Items)
        {
            var proto = itemManager.GetPrototype(item.ItemId);
            if (proto != null && proto.ItemType != ItemType.Quest)
            {
                _shopItems.Add(new ShopItemInstance
                {
                    Prototype = proto,
                    PriceModifier = item.PriceModifier,
                    AvailableQuantity = item.Quantity
                });
            }
        }

        _hints = new List<(string, ConsoleColor)>
        {
            ("↑/↓ - выбор", ConsoleColor.Gray),
            ("←/→ - смена режима (купить/продать)", ConsoleColor.Gray),
            ("+/- - изменить количество", ConsoleColor.Gray),
            ("Enter - совершить сделку", ConsoleColor.Gray),
            ("Backspace - закрыть", ConsoleColor.Gray)
        };
    }

    public override void Render()
    {
        UIManager.Clear();

        UIManager.DrawString(2, 2, _shop.Name, ConsoleColor.Yellow);
        UIManager.DrawString(2, 3, new string('=', 50), ConsoleColor.DarkGray);

        var reputationManager = DI.Resolve<ReputationManager>();
        double priceMultiplier = reputationManager.GetPriceModifier(_shop.Faction);

        UIManager.DrawString(2, 4, $"Кредиты: {_player.Credits}", ConsoleColor.Green);
        UIManager.DrawString(2, 5, _buyingMode ? "РЕЖИМ: ПОКУПКА" : "РЕЖИМ: ПРОДАЖА", _buyingMode ? ConsoleColor.Cyan : ConsoleColor.Magenta);
        UIManager.DrawString(2, 6, $"Количество: {_selectedQuantity}", ConsoleColor.Gray);
        if (_shop.Faction.HasValue)
        {
            string factionName = _shop.Faction.Value.ToString();
            UIManager.DrawString(2, 7, $"Фракция: {factionName} (множитель цен: {priceMultiplier:F2})", ConsoleColor.DarkYellow);
        }

        int startY = 9;
        if (_buyingMode)
        {
            for (int i = 0; i < ItemsPerPage; i++)
            {
                int idx = _scrollOffset + i;
                if (idx >= _shopItems.Count) break;

                var item = _shopItems[idx];
                int price = (int)(item.Prototype.BasePrice * priceMultiplier * item.PriceModifier);
                string quantityStr = item.AvailableQuantity == -1 ? "∞" : item.AvailableQuantity.ToString();
                string line = $"{item.Prototype.Name} - {price} кр [в наличии: {quantityStr}]";
                if (idx == _selectedIndex)
                    UIManager.DrawString(2, startY + i, "> " + line, ConsoleColor.Yellow);
                else
                    UIManager.DrawString(2, startY + i, "  " + line, ConsoleColor.White);
            }
        }
        else
        {
            var playerItems = _player.Inventory.Items
                .Where(i => i.Prototype.ItemType != ItemType.Quest)
                .ToList();
            for (int i = 0; i < ItemsPerPage; i++)
            {
                int idx = _scrollOffset + i;
                if (idx >= playerItems.Count) break;

                var item = playerItems[idx];
                int price = (int)(item.Prototype.BasePrice * 0.5 * priceMultiplier);
                string line = $"{item.Prototype.Name} x{item.Quantity} - {price} кр";
                if (idx == _selectedIndex)
                    UIManager.DrawString(2, startY + i, "> " + line, ConsoleColor.Yellow);
                else
                    UIManager.DrawString(2, startY + i, "  " + line, ConsoleColor.White);
            }
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
                int maxIndex = _buyingMode ? _shopItems.Count - 1 : _player.Inventory.Items.Count(i => i.Prototype.ItemType != ItemType.Quest) - 1;
                if (_selectedIndex < maxIndex)
                {
                    _selectedIndex++;
                    if (_selectedIndex >= _scrollOffset + ItemsPerPage)
                        _scrollOffset = _selectedIndex - ItemsPerPage + 1;
                }
                return true;
            case ConsoleKey.LeftArrow:
            case ConsoleKey.RightArrow:
                _buyingMode = !_buyingMode;
                _selectedIndex = 0;
                _scrollOffset = 0;
                return true;
            case ConsoleKey.OemPlus:
            case ConsoleKey.Add:
                _selectedQuantity++;
                return true;
            case ConsoleKey.OemMinus:
            case ConsoleKey.Subtract:
                if (_selectedQuantity > 1) _selectedQuantity--;
                return true;
            case ConsoleKey.Enter:
                if (_buyingMode)
                    BuyItem();
                else
                    SellItem();
                return true;
            case ConsoleKey.Backspace:
                ScreenManager.PopScreen();
                return true;
            default:
                return false;
        }
    }

    private void BuyItem()
    {
        if (_selectedIndex >= _shopItems.Count) return;
        var item = _shopItems[_selectedIndex];
        if (item.AvailableQuantity == 0) return;

        var reputationManager = DI.Resolve<ReputationManager>();
        double priceMultiplier = reputationManager.GetPriceModifier(_shop.Faction);
        int price = (int)(item.Prototype.BasePrice * priceMultiplier * item.PriceModifier);

        int maxCanBuy = _player.Credits / price;
        if (item.AvailableQuantity != -1 && item.AvailableQuantity < maxCanBuy)
            maxCanBuy = item.AvailableQuantity;

        int quantity = Math.Min(_selectedQuantity, maxCanBuy);
        if (quantity <= 0)
        {
            UIManager.ShowMessage("Недостаточно кредитов или товара нет в наличии");
            return;
        }

        double totalWeight = item.Prototype.Weight * quantity + _player.GetTotalWeight();
        if (totalWeight > _player.GetMaxWeight())
        {
            UIManager.ShowMessage("Слишком тяжело!");
            return;
        }

        _player.Credits -= price * quantity;
        var newItem = new ItemInstance(item.Prototype.Id, quantity);
        newItem.Prototype = item.Prototype;
        _player.Inventory.AddItem(newItem);

        if (item.AvailableQuantity != -1)
            item.AvailableQuantity -= quantity;

        UIManager.ShowMessage($"Куплено {quantity} {item.Prototype.Name}");
    }

    private void SellItem()
    {
        var playerItems = _player.Inventory.Items
            .Where(i => i.Prototype.ItemType != ItemType.Quest)
            .ToList();
        if (_selectedIndex >= playerItems.Count) return;

        var item = playerItems[_selectedIndex];
        var reputationManager = DI.Resolve<ReputationManager>();
        double priceMultiplier = reputationManager.GetPriceModifier(_shop.Faction);
        int price = (int)(item.Prototype.BasePrice * 0.5 * priceMultiplier);

        int quantity = Math.Min(_selectedQuantity, item.Quantity);
        if (quantity <= 0) return;

        _player.Credits += price * quantity;
        _player.Inventory.RemoveItem(item, quantity);

        UIManager.ShowMessage($"Продано {quantity} {item.Prototype.Name}");
    }

    public override List<(string text, ConsoleColor color)> GetHints() => _hints;
}