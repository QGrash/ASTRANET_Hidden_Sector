using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public class DoorEntity : InteriorEntity
    {
        public bool IsLocked { get; set; }
        public bool IsOpen { get; set; }

        public DoorEntity(string name, bool locked, int x, int y)
        {
            Name = name;
            IsLocked = locked;
            IsOpen = false;
            X = x;
            Y = y;
            IsSolid = true; // закрытая дверь блокирует проход
            UpdateSymbol();
        }

        private void UpdateSymbol()
        {
            if (IsOpen)
            {
                Symbol = '░';
                Color = ConsoleColor.DarkYellow;
                IsSolid = false;
            }
            else if (IsLocked)
            {
                Symbol = '▒';
                Color = ConsoleColor.Red;
                IsSolid = true;
            }
            else
            {
                Symbol = '▒';
                Color = ConsoleColor.Yellow;
                IsSolid = true;
            }
        }

        public override void Interact(GameStateManager stateManager, UIManager uiManager)
        {
            if (IsLocked)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Дверь", "Дверь заперта."));
            }
            else if (IsOpen)
            {
                // Закрыть дверь
                IsOpen = false;
                UpdateSymbol();
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Дверь", "Вы закрыли дверь."));
            }
            else
            {
                // Открыть дверь
                IsOpen = true;
                UpdateSymbol();
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Дверь", "Вы открыли дверь."));
            }
        }
    }
}