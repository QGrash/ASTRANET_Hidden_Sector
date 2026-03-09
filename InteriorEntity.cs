using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Screens;
using ASTRANET_Hidden_Sector.World;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public abstract class InteriorEntity : MapEntity
    {
        public string InteractDescription { get; set; } = "";
        public bool IsSolid { get; set; } = true;

        public override void Interact(GameStateManager stateManager, UIManager uiManager)
        {
            stateManager.PushScreen(new MessageScreen(stateManager, uiManager, GetType().Name, InteractDescription));
        }
    }
}