using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueChoiceData
    {
        public string ChoiceText { get; set; } = "";
        public string NextNodeId { get; set; } = "";
        public List<DialogueConditionData> Conditions { get; set; } = new();
        public List<DialogueActionData> Actions { get; set; } = new();
    }
}