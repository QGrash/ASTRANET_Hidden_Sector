using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public class Dialogue
    {
        public string Id { get; set; }
        public string NpcName { get; set; }
        public Dictionary<string, DialogueNode> Nodes { get; set; }
        public string StartNodeId { get; set; }

        public Dialogue(DialogueData data)
        {
            Id = data.Id;
            NpcName = data.NpcName;
            StartNodeId = data.StartNodeId;
            Nodes = new Dictionary<string, DialogueNode>();

            foreach (var kv in data.Nodes)
            {
                var node = new DialogueNode
                {
                    Id = kv.Value.Id,
                    NpcText = kv.Value.NpcText,
                    Choices = new List<DialogueChoice>()
                };

                foreach (var choiceData in kv.Value.Choices)
                {
                    var choice = new DialogueChoice
                    {
                        ChoiceText = choiceData.ChoiceText,
                        NextNodeId = choiceData.NextNodeId,
                        Conditions = new List<DialogueCondition>(),
                        Actions = new List<DialogueAction>()
                    };
                    // Преобразование условий и действий (пока просто заглушка)
                    node.Choices.Add(choice);
                }

                Nodes[kv.Key] = node;
            }
        }
    }
}