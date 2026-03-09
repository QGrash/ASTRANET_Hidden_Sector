using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public class ContainerEntity : InteriorEntity
    {
        public List<string> Loot { get; set; } = new();
        public bool IsLocked { get; set; }
        public bool IsOpen { get; set; }

        public ContainerEntity(string name, bool locked, List<string> loot, int x, int y)
        {
            Name = name;
            IsLocked = locked;
            Loot = loot;
            X = x;
            Y = y;
            IsOpen = false;
            IsSolid = false;
            Symbol = 'X';
            Color = ConsoleColor.DarkYellow;
        }

        public override void Interact(GameStateManager stateManager, UIManager uiManager)
        {
            if (IsLocked)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Контейнер", "Контейнер заперт."));
            }
            else if (IsOpen)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Контейнер", "Контейнер пуст."));
            }
            else
            {
                IsOpen = true;
                string lootList = Loot.Count > 0 ? string.Join(", ", Loot) : "ничего нет";
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Контейнер",
                    $"Вы открыли контейнер. Внутри: {lootList}."));
                // TODO: добавить предметы в инвентарь
            }
        }
    }
}