using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueNode
    {
        public string Id { get; set; } = "";
        public string NpcText { get; set; } = "";
        public List<DialogueChoice> Choices { get; set; } = new();
        public bool IsEnd { get; set; } = false;
    }
}