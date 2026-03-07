using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Dialogue
{
    public static class DialogueManager
    {
        private static Dictionary<string, Dialogue> dialogues = new();

        static DialogueManager()
        {
            DialogueLoader.LoadAll();
            // Преобразуем загруженные данные в runtime-диалоги
            foreach (var kv in DialogueLoader.GetAll()) // нужно добавить метод GetAll в DialogueLoader
            {
                dialogues[kv.Key] = new Dialogue(kv.Value);
            }
        }

        public static Dialogue GetDialogue(string id)
        {
            return dialogues.ContainsKey(id) ? dialogues[id] : null;
        }
    }
}