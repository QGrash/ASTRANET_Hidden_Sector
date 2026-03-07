using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Faction;
using ASTRANET_Hidden_Sector.Entities.Quest;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueCondition
    {
        public string Type { get; set; } = "";
        public string Target { get; set; } = "";
        public int Value { get; set; }

        public bool IsMet()
        {
            switch (Type)
            {
                case "hasItem":
                    return Game.CurrentPlayer?.Inventory?.FindItem(Target) != null;

                case "hasCredits":
                    return Game.CurrentPlayer?.Credits >= Value;

                case "reputationMin":
                    int rep = FactionManager.GetReputation(Target);
                    return rep >= Value;

                case "reputationMax":
                    rep = FactionManager.GetReputation(Target);
                    return rep <= Value;

                case "questActive":
                    var quest = QuestManager.GetQuest(Target);
                    return quest != null && quest.State == QuestState.Active;

                case "questCompleted":
                    var q = QuestManager.GetQuest(Target);
                    return q != null && q.State == QuestState.Completed;

                case "questNotStarted":
                    var qns = QuestManager.GetQuest(Target);
                    return qns == null || qns.State == QuestState.NotStarted;

                case "statCheck":
                    if (Game.CurrentPlayer != null)
                    {
                        switch (Target)
                        {
                            case "Strength": return Game.CurrentPlayer.Strength >= Value;
                            case "Dexterity": return Game.CurrentPlayer.Dexterity >= Value;
                            case "Intelligence": return Game.CurrentPlayer.Intelligence >= Value;
                            case "Luck": return Game.CurrentPlayer.Luck >= Value;
                            case "Charisma": return Game.CurrentPlayer.Charisma >= Value;
                            default: return false;
                        }
                    }
                    return false;

                default:
                    return true;
            }
        }
    }
}