using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.World
{
    public class StaticEntity : MapEntity
    {
        public LocationType Type { get; set; }
        public string DialogueId { get; set; } = "";

        public override void Interact(GameStateManager stateManager, UIManager uiManager)
        {
            if (Type == LocationType.Station && !string.IsNullOrEmpty(DialogueId))
            {
                stateManager.PushScreen(new DialogueScreen(stateManager, uiManager, DialogueId));
            }
            else if (Type == LocationType.Resources)
            {
                CollectResources(stateManager, uiManager);
            }
            else
            {
                string title = Type.ToString();
                string desc = Description;
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, title, desc));
            }
        }

        private void CollectResources(GameStateManager stateManager, UIManager uiManager)
        {
            var player = Game.CurrentPlayer;
            if (player == null) return;

            Item? item = null;
            if (Name.Contains("Топливо") || Name.Contains("Fuel"))
                item = new Item("fuel", "Топливо", "Канистра с топливом", ItemType.Resource, 10f, 50);
            else if (Name.Contains("Металл") || Name.Contains("Metal"))
                item = new Item("metal", "Металл", "Слиток металла", ItemType.Resource, 5f, 30);
            else
                item = new Item("scrap", "Запчасти", "Разные запчасти", ItemType.Resource, 2f, 20);

            if (item != null)
            {
                if (player.Inventory.AddItem(item))
                {
                    stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Сбор ресурсов",
                        $"Вы собрали {item.Name}. Вес: {item.Weight} кг."));
                    IsDestroyed = true;
                }
                else
                {
                    stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Перегруз!",
                        $"Недостаточно места в инвентаре. Нужно {item.Weight} кг, доступно {player.Inventory.MaxWeight - player.Inventory.CurrentWeight} кг."));
                }
            }
        }

        public StaticEntity(Location loc, string dialogueId = "")
        {
            Name = loc.Name;
            Description = loc.Description;
            Type = loc.Type;
            X = loc.X;
            Y = loc.Y;
            DialogueId = dialogueId;

            switch (Type)
            {
                case LocationType.Planet:
                    Symbol = 'O';
                    Color = ConsoleColor.Green;
                    IsStatic = true; // планета – статика
                    break;
                case LocationType.Station:
                    Symbol = 'H';
                    Color = ConsoleColor.Cyan;
                    IsStatic = true; // станция – статика
                    break;
                case LocationType.Enemy:
                    Symbol = 'Y';
                    Color = ConsoleColor.Red;
                    IsStatic = false; // враг – корабль (не статика)
                    break;
                case LocationType.Anomaly:
                    Symbol = '~';
                    Color = ConsoleColor.Magenta;
                    IsStatic = true; // аномалия – статика
                    break;
                case LocationType.Resources:
                    Symbol = '$';
                    Color = ConsoleColor.Yellow;
                    IsStatic = true; // ресурсы – статика? Они исчезают, но не двигаются. Пока оставим true.
                    break;
                default:
                    Symbol = '·';
                    Color = ConsoleColor.DarkGray;
                    IsStatic = true;
                    break;
            }
        }
    }
}