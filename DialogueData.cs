using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class DialogueData
    {
        public string Id { get; set; } = "";
        public string NpcName { get; set; } = "";
        public string StartNodeId { get; set; } = "";
        public Dictionary<string, DialogueNodeData> Nodes { get; set; } = new();
    }
}