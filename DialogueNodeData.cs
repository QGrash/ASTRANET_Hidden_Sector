using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueNodeData
    {
        public string Id { get; set; } = "";
        public string NpcText { get; set; } = "";
        public List<DialogueChoiceData> Choices { get; set; } = new();
    }
}