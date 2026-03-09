using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities;
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

            string header = "=== ТОРГОВЛЯ ===";
            for (int i = 0; i < header.Length; i++)
                uiManager.SetPixel(leftX + i, topY - 1, header[i], ConsoleColor.Yellow);

            string credits = $"Кредиты: {player.Credits}";
            for (int i = 0; i < credits.Length; i++)
                uiManager.SetPixel(leftX + 20 + i, topY - 1, credits[i], ConsoleColor.Cyan);

            string buyTab = currentMode == ShopMode.Buying ? "[Покупка]  " : " Покупка   ";
            string sellTab = currentMode == ShopMode.Selling ? "[Продажа]" : " Продажа ";

            for (int i = 0; i < buyTab.Length; i++)
                uiManager.SetPixel(leftX + i, topY, buyTab[i],
                    currentMode == ShopMode.Buying ? ConsoleColor.Cyan : ConsoleColor.DarkGray);
            for (int i = 0; i < sellTab.Length; i++)
                uiManager.SetPixel(leftX + 12 + i, topY, sellTab[i],
                    currentMode == ShopMode.Selling ? ConsoleColor.Cyan : ConsoleColor.DarkGray);

            string stationHeader = "Товары станции";
            for (int i = 0; i < stationHeader.Length; i++)
                uiManager.SetPixel(leftX + i, topY + 2, stationHeader[i], ConsoleColor.Green);
            string playerHeader = "Ваш инвентарь";
            for (int i = 0; i < playerHeader.Length; i++)
                uiManager.SetPixel(leftX + 40 + i, topY + 2, playerHeader[i], ConsoleColor.Green);

            int listStartY = topY + 4;
            int rep = FactionManager.GetReputation(stationInventory.FactionId);

            for (int i = 0; i < stationGoods.Count; i++)
            {
                var good = stationGoods[i];
                var stationItem = stationInventory.Items.Find(ii => ii.GoodId == good.Id);
                int price = stationInventory.GetBuyPrice(good, rep);

                int y = listStartY + i;
                if (currentMode == ShopMode.Buying && i == selectedStationIndex)
                    uiManager.SetPixel(leftX, y, '>', ConsoleColor.Yellow);
                else
                    uiManager.SetPixel(leftX, y, ' ', ConsoleColor.Black);

                string line = $"{good.Name} ({stationItem.Quantity} шт.) - {price} кр.";
                for (int j = 0; j < line.Length; j++)
                    uiManager.SetPixel(leftX + 2 + j, y, line[j], ConsoleColor.White);
            }

            for (int i = 0; i < playerGoods.Count; i++)
            {
                var good = playerGoods[i];
                int price = stationInventory.GetSellPrice(good, rep);

                int y = listStartY + i;
                if (currentMode == ShopMode.Selling && i == selectedPlayerIndex)
                    uiManager.SetPixel(leftX + 40, y, '>', ConsoleColor.Yellow);
                else
                    uiManager.SetPixel(leftX + 40, y, ' ', ConsoleColor.Black);

                string line = $"{good.Name} - {price} кр.";
                for (int j = 0; j < line.Length; j++)
                    uiManager.SetPixel(leftX + 42 + j, y, line[j], ConsoleColor.White);
            }

            string quantityLine = "";
            if (currentMode == ShopMode.Buying && stationGoods.Count > 0)
                quantityLine = $"Количество для покупки: {quantity} (←/→ изменить)";
            else if (currentMode == ShopMode.Selling && playerGoods.Count > 0)
                quantityLine = $"Количество для продажи: {quantity} (←/→ изменить)";

            if (!string.IsNullOrEmpty(quantityLine))
            {
                for (int i = 0; i < quantityLine.Length; i++)
                    uiManager.SetPixel(2 + i, Console.WindowHeight - 3, quantityLine[i], ConsoleColor.DarkGray);
            }

            string hint = "Tab - переключить режим, Enter - совершить сделку, Esc - выход";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
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