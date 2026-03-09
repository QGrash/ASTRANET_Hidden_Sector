using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Faction;
using ASTRANET_Hidden_Sector.Entities.Interior;
using ASTRANET_Hidden_Sector.Entities.Quest;
using ASTRANET_Hidden_Sector.Screens;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueAction
    {
        public string Type { get; set; } = "";
        public string Target { get; set; } = "";
        public int Value { get; set; }

        public void Execute(GameStateManager stateManager, UIManager uiManager, NpcEntity? npc = null)
        {
            switch (Type)
            {
                case "openShop":
                    if (npc != null && npc.TraderInventory != null)
                    {
                        stateManager.PushScreen(new ShopScreen(stateManager, uiManager, npc.TraderInventory));
                    }
                    else
                    {
                        stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Магазин",
                            "Этот персонаж ничего не продаёт."));
                    }
                    break;

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

                case "enterInterior":
                    var template = InteriorLoader.GetTemplate(Target);
                    if (template != null)
                    {
                        var interiorMap = InteriorMap.LoadFromTemplate(template);
                        stateManager.PushScreen(new InteriorMapScreen(stateManager, uiManager, interiorMap, template.Name));
                    }
                    else
                    {
                        stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Ошибка", $"Интерьер {Target} не найден."));
                    }
                    break;

                default:
                    break;
            }
        }
    }
}