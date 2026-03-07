using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Faction;
using ASTRANET_Hidden_Sector.Entities.Quest;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueAction
    {
        public string Type { get; set; } = "";
        public string Target { get; set; } = "";
        public int Value { get; set; }

        public void Execute(GameStateManager stateManager, UIManager uiManager)
        {
            switch (Type)
            {
                case "openShop":
                    stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Магазин", "Здесь будет торговля."));
                    break;
                    
                    stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Магазин", "Здесь будет торговля."));

                case "addCredits":
                    if (Game.CurrentPlayer != null)
                        Game.CurrentPlayer.Credits += Value;
                    break;

                case "removeCredits":
                    if (Game.CurrentPlayer != null && Game.CurrentPlayer.Credits >= Value)
                        Game.CurrentPlayer.Credits -= Value;
                    break;

                case "changeReputation":
                    FactionManager.ModifyReputation(Target, Value);
                    break;

                case "startQuest":
                    QuestManager.StartQuest(Target);
                    break;

                case "updateQuest":
                    QuestManager.UpdateProgress(ObjectiveType.CollectItem, Target, Value);
                    break;

                case "completeQuest":
                    QuestManager.UpdateProgress(ObjectiveType.CollectItem, Target, 999);
                    break;

                default:
                    break;
            }
        }
    }
}