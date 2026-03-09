using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public class ConsoleEntity : InteriorEntity
    {
        public string Action { get; set; } = "";

        public ConsoleEntity(string name, string description, string action, int x, int y)
        {
            Name = name;
            InteractDescription = description;
            Action = action;
            X = x;
            Y = y;
            IsSolid = false;
            Symbol = 'C';
            Color = ConsoleColor.Green;
        }

        public override void Interact(GameStateManager stateManager, UIManager uiManager)
        {
            stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Консоль",
                $"Вы активировали {Name}. Действие: {Action}"));
        }
    }
}