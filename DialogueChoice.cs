using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueChoice
    {
        public string ChoiceText { get; set; } = "";
        public string NextNodeId { get; set; } = "";
        public List<DialogueCondition> Conditions { get; set; } = new();
        public List<DialogueAction> Actions { get; set; } = new();
    }
}