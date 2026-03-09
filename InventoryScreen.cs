using System;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class InventoryScreen : GameScreen
    {
        private int selectedIndex = 0;
        private Player player;

        public InventoryScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
            player = Game.CurrentPlayer!;
        }

        public override void Render()
        {
            uiManager.Clear();

            int leftX = 5;
            int topY = 3;
            var items = player.Inventory.GetAllItems();

            string header = "ИНВЕНТАРЬ";
            for (int i = 0; i < header.Length; i++)
                uiManager.SetPixel(leftX + i, topY - 2, header[i], ConsoleColor.Yellow);

            string weightInfo = $"Вес: {player.Inventory.CurrentWeight}/{player.Inventory.MaxWeight} кг";
            for (int i = 0; i < weightInfo.Length; i++)
                uiManager.SetPixel(leftX + 15 + i, topY - 2, weightInfo[i], ConsoleColor.Cyan);

            if (items.Count == 0)
            {
                string empty = "Инвентарь пуст";
                for (int i = 0; i < empty.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, empty[i], ConsoleColor.DarkGray);
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    int y = topY + i;
                    if (i == selectedIndex)
                        uiManager.SetPixel(leftX, y, '>', ConsoleColor.Yellow);
                    else
                        uiManager.SetPixel(leftX, y, ' ', ConsoleColor.Black);

                    var item = items[i];
                    string line = $"{item.Name} ({item.Weight} кг)";
                    if (item.Type == ItemType.Consumable && item.HealAmount > 0)
                        line += $" [лечит {item.HealAmount}]";
                    for (int j = 0; j < line.Length; j++)
                        uiManager.SetPixel(leftX + 2 + j, y, line[j], ConsoleColor.White);
                }
            }

            if (items.Count > 0 && selectedIndex >= 0 && selectedIndex < items.Count)
            {
                var item = items[selectedIndex];
                int infoX = leftX + 40;
                int infoY = topY;

                string desc = $"Описание: {item.Description}";
                for (int i = 0; i < desc.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, desc[i], ConsoleColor.Gray);
                infoY++;

                string type = $"Тип: {item.Type}";
                for (int i = 0; i < type.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, type[i], ConsoleColor.Green);
                infoY++;

                string price = $"Цена: {item.Value} кредитов";
                for (int i = 0; i < price.Length; i++)
                    uiManager.SetPixel(infoX + i, infoY, price[i], ConsoleColor.Yellow);
            }

            string hint = "↑/↓ - выбор, Enter - использовать, D - выбросить, Backspace - назад, ESC - меню";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            var items = player.Inventory.GetAllItems();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (items.Count > 0)
                        selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
                    break;
                case ConsoleKey.DownArrow:
                    if (items.Count > 0)
                        selectedIndex = (selectedIndex + 1) % items.Count;
                    break;
                case ConsoleKey.Enter:
                    if (items.Count > 0)
                        UseItem(items[selectedIndex]);
                    break;
                case ConsoleKey.D:
                    if (items.Count > 0)
                        DropItem(items[selectedIndex]);
                    break;
                case ConsoleKey.Backspace:
                    stateManager.PopScreen();
                    break;
            }
        }

        private void UseItem(Item item)
        {
            if (item.Type == ItemType.Consumable && item.HealAmount > 0)
            {
                player.Health = Math.Min(player.Health + item.HealAmount, player.MaxHealth);
                player.Inventory.RemoveItem(item);
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Использован",
                    $"Вы использовали {item.Name}. Здоровье: {player.Health}/{player.MaxHealth}"));
            }
            else
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Нельзя использовать",
                    "Этот предмет нельзя использовать напрямую."));
            }
        }

        private void DropItem(Item item)
        {
            string message = $"Вы уверены, что хотите выбросить {item.Name}?";
            stateManager.PushScreen(new ConfirmScreen(stateManager, uiManager, message, (confirmed) =>
            {
                if (confirmed)
                {
                    player.Inventory.RemoveItem(item);
                    if (player.Inventory.GetAllItems().Count == 0)
                        selectedIndex = 0;
                    else if (selectedIndex >= player.Inventory.GetAllItems().Count)
                        selectedIndex = player.Inventory.GetAllItems().Count - 1;
                }
            }));
        }
    }
}