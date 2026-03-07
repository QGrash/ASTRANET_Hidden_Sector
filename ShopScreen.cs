using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities; // добавлено для StationInventory
using ASTRANET_Hidden_Sector.Entities.Faction;
using ASTRANET_Hidden_Sector.Entities.Trade;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class ShopScreen : GameScreen
    {
        private enum ShopMode { Buying, Selling }
        private ShopMode currentMode = ShopMode.Buying;

        private StationInventory stationInventory;
        private Player player;
        private int selectedStationIndex = 0;
        private int selectedPlayerIndex = 0;
        private int quantity = 1;

        private List<TradeGood> stationGoods;
        private List<TradeGood> playerGoods;

        public ShopScreen(GameStateManager stateManager, UIManager uiManager, StationInventory inventory)
            : base(stateManager, uiManager)
        {
            stationInventory = inventory;
            player = Game.CurrentPlayer!;

            RefreshGoods();
        }

        private void RefreshGoods()
        {
            stationGoods = stationInventory.Items
                .Where(item => item.Quantity > 0)
                .Select(item => TradeGoodsLoader.GetGood(item.GoodId))
                .Where(g => g != null)
                .ToList();

            playerGoods = player.Inventory.GetAllItems()
                .Select(i => TradeGoodsLoader.GetGood(i.Id))
                .Where(g => g != null)
                .ToList();
        }

        public override void Render()
        {
            uiManager.Clear();

            int leftX = 5;
            int topY = 3;

            uiManager.SetCursorPosition(leftX, topY - 1);
            uiManager.Write("=== ТОРГОВЛЯ ===", ConsoleColor.Yellow);
            uiManager.SetCursorPosition(leftX + 20, topY - 1);
            uiManager.Write($"Кредиты: {player.Credits}", ConsoleColor.Cyan);

            uiManager.SetCursorPosition(leftX, topY);
            if (currentMode == ShopMode.Buying)
                uiManager.Write("[Покупка]  ", ConsoleColor.Cyan);
            else
                uiManager.Write(" Покупка   ", ConsoleColor.DarkGray);

            uiManager.SetCursorPosition(leftX + 12, topY);
            if (currentMode == ShopMode.Selling)
                uiManager.Write("[Продажа]", ConsoleColor.Cyan);
            else
                uiManager.Write(" Продажа ", ConsoleColor.DarkGray);

            uiManager.SetCursorPosition(leftX, topY + 2);
            uiManager.Write("Товары станции", ConsoleColor.Green);
            uiManager.SetCursorPosition(leftX + 40, topY + 2);
            uiManager.Write("Ваш инвентарь", ConsoleColor.Green);

            int listStartY = topY + 4;
            int rep = FactionManager.GetReputation(stationInventory.FactionId);

            for (int i = 0; i < stationGoods.Count; i++)
            {
                var good = stationGoods[i];
                var stationItem = stationInventory.Items.Find(ii => ii.GoodId == good.Id);
                int price = stationInventory.GetBuyPrice(good, rep);

                uiManager.SetCursorPosition(leftX, listStartY + i);
                if (currentMode == ShopMode.Buying && i == selectedStationIndex)
                    uiManager.Write("> ", ConsoleColor.Yellow);
                else
                    uiManager.Write("  ", ConsoleColor.Gray);

                uiManager.Write($"{good.Name} ({stationItem.Quantity} шт.) - {price} кр.", ConsoleColor.White);
            }

            for (int i = 0; i < playerGoods.Count; i++)
            {
                var good = playerGoods[i];
                int price = stationInventory.GetSellPrice(good, rep);

                uiManager.SetCursorPosition(leftX + 40, listStartY + i);
                if (currentMode == ShopMode.Selling && i == selectedPlayerIndex)
                    uiManager.Write("> ", ConsoleColor.Yellow);
                else
                    uiManager.Write("  ", ConsoleColor.Gray);

                uiManager.Write($"{good.Name} - {price} кр.", ConsoleColor.White);
            }

            uiManager.SetCursorPosition(2, Console.WindowHeight - 3);
            if (currentMode == ShopMode.Buying && stationGoods.Count > 0)
            {
                uiManager.Write($"Количество для покупки: {quantity} (←/→ изменить)", ConsoleColor.DarkGray);
            }
            else if (currentMode == ShopMode.Selling && playerGoods.Count > 0)
            {
                uiManager.Write($"Количество для продажи: {quantity} (←/→ изменить)", ConsoleColor.DarkGray);
            }

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("Tab - переключить режим, Enter - совершить сделку, Esc - выход", ConsoleColor.DarkGray);
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentMode == ShopMode.Buying && stationGoods.Count > 0)
                        selectedStationIndex = (selectedStationIndex - 1 + stationGoods.Count) % stationGoods.Count;
                    else if (currentMode == ShopMode.Selling && playerGoods.Count > 0)
                        selectedPlayerIndex = (selectedPlayerIndex - 1 + playerGoods.Count) % playerGoods.Count;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentMode == ShopMode.Buying && stationGoods.Count > 0)
                        selectedStationIndex = (selectedStationIndex + 1) % stationGoods.Count;
                    else if (currentMode == ShopMode.Selling && playerGoods.Count > 0)
                        selectedPlayerIndex = (selectedPlayerIndex + 1) % playerGoods.Count;
                    break;
                case ConsoleKey.LeftArrow:
                    if (quantity > 1) quantity--;
                    break;
                case ConsoleKey.RightArrow:
                    quantity++;
                    break;
                case ConsoleKey.Tab:
                    currentMode = currentMode == ShopMode.Buying ? ShopMode.Selling : ShopMode.Buying;
                    quantity = 1;
                    break;
                case ConsoleKey.Enter:
                    ExecuteTrade();
                    break;
                case ConsoleKey.Escape:
                    stateManager.PopScreen();
                    break;
            }
        }

        private void ExecuteTrade()
        {
            if (currentMode == ShopMode.Buying && stationGoods.Count > 0)
            {
                BuyItem();
            }
            else if (currentMode == ShopMode.Selling && playerGoods.Count > 0)
            {
                SellItem();
            }
        }

        private void BuyItem()
        {
            var good = stationGoods[selectedStationIndex];
            var stationItem = stationInventory.Items.Find(ii => ii.GoodId == good.Id);
            if (stationItem == null) return;

            int rep = FactionManager.GetReputation(stationInventory.FactionId);
            int price = stationInventory.GetBuyPrice(good, rep);
            int totalCost = price * quantity;

            if (player.Credits < totalCost)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Недостаточно кредитов",
                    $"Вам нужно {totalCost} кредитов."));
                return;
            }

            if (stationItem.Quantity < quantity)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Недостаточно товара",
                    $"На станции только {stationItem.Quantity} шт."));
                return;
            }

            float totalWeight = good.Weight * quantity;
            if (player.Inventory.CurrentWeight + totalWeight > player.Inventory.MaxWeight)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Перегруз",
                    "Недостаточно места в инвентаре."));
                return;
            }

            player.Credits -= totalCost;
            stationItem.Quantity -= quantity;

            for (int i = 0; i < quantity; i++)
            {
                var item = new Item(good.Id, good.Name, good.Description, ItemType.Misc, good.Weight, good.BasePrice);
                player.Inventory.AddItem(item);
            }

            RefreshGoods();
            quantity = 1;
        }

        private void SellItem()
        {
            var good = playerGoods[selectedPlayerIndex];
            var playerItem = player.Inventory.GetAllItems().FirstOrDefault(i => i.Id == good.Id);
            if (playerItem == null) return;

            int rep = FactionManager.GetReputation(stationInventory.FactionId);
            int price = stationInventory.GetSellPrice(good, rep);
            int totalIncome = price * quantity;

            var itemsWithId = player.Inventory.GetAllItems().Where(i => i.Id == good.Id).ToList();
            if (itemsWithId.Count < quantity)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Недостаточно товара",
                    $"У вас только {itemsWithId.Count} шт."));
                return;
            }

            var stationItem = stationInventory.Items.Find(ii => ii.GoodId == good.Id);
            if (stationItem == null)
            {
                stationItem = new StationInventory.InventoryItem
                {
                    GoodId = good.Id,
                    Quantity = 0,
                    PriceModifier = 0
                };
                stationInventory.Items.Add(stationItem);
            }

            player.Credits += totalIncome;
            stationItem.Quantity += quantity;

            for (int i = 0; i < quantity; i++)
            {
                player.Inventory.RemoveItem(playerItem);
            }

            RefreshGoods();
            quantity = 1;
        }
    }
}