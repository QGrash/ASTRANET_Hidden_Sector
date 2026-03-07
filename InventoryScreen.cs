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

            int leftX = 10;
            int topY = 5;
            var items = player.Inventory.GetAllItems();

            uiManager.SetCursorPosition(leftX, topY - 2);
            uiManager.Write($"ИНВЕНТАРЬ", ConsoleColor.Yellow);
            uiManager.SetCursorPosition(leftX + 20, topY - 2);
            uiManager.Write($"Вес: {player.Inventory.CurrentWeight}/{player.Inventory.MaxWeight} кг", ConsoleColor.Cyan);

            if (items.Count == 0)
            {
                uiManager.SetCursorPosition(leftX, topY);
                uiManager.Write("Инвентарь пуст", ConsoleColor.DarkGray);
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    uiManager.SetCursorPosition(leftX, topY + i);
                    if (i == selectedIndex)
                        uiManager.Write("> ", ConsoleColor.Yellow);
                    else
                        uiManager.Write("  ", ConsoleColor.Gray);

                    var item = items[i];
                    string line = $"{item.Name} ({item.Weight} кг)";
                    if (item.Type == ItemType.Consumable && item.HealAmount > 0)
                        line += $" [лечит {item.HealAmount}]";
                    uiManager.Write(line, ConsoleColor.White);
                }
            }

            if (items.Count > 0 && selectedIndex >= 0 && selectedIndex < items.Count)
            {
                var item = items[selectedIndex];
                int infoX = leftX + 40;
                int infoY = topY;

                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write($"Описание: {item.Description}", ConsoleColor.Gray);
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write($"Тип: {item.Type}", ConsoleColor.Green);
                uiManager.SetCursorPosition(infoX, infoY++);
                uiManager.Write($"Цена: {item.Value} кредитов", ConsoleColor.Yellow);
            }

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("↑/↓ - выбор, Enter - использовать, D - выбросить, Backspace - назад, ESC - меню", ConsoleColor.DarkGray);
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