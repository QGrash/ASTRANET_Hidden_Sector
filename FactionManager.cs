using System.Collections.Generic;
using System.Linq;

namespace ASTRANET_Hidden_Sector.Entities.Faction
{
    public static class FactionManager
    {
        private static Dictionary<string, Faction> factions = new Dictionary<string, Faction>();
        private static Dictionary<string, int> reputation = new Dictionary<string, int>();

        static FactionManager()
        {
            LoadFactions();
        }

        private static void LoadFactions()
        {
            factions["federation"] = new Faction("federation", "Солнечная Федерация", "Единое государство людей и других рас.", 0);
            factions["piro_tetta"] = new Faction("piro_tetta", "Империя Пиро-Тэтта", "Милитаристская империя пиролэков.", -20);
            factions["aqua_tarris"] = new Faction("aqua_tarris", "Республика Аква-Таррис", "Торговая республика найминов.", 10);
            factions["brog"] = new Faction("brog", "Союз Кочевников Брог", "Агрессивные кочевники зед'брог.", -30);
            factions["green_cross"] = new Faction("green_cross", "Галактический фонд Зелёного Креста", "Гуманитарная организация.", 20);
            factions["merc_armory"] = new Faction("merc_armory", "Корпорация Мерк-армори", "Наёмники и торговцы оружием.", 0);
            factions["cult_singo"] = new Faction("cult_singo", "Культ Бездны Синго", "Религиозные фанатики.", -50);
            factions["tarrshak"] = new Faction("tarrshak", "Тарр'шак Аква", "Торговая корпорация, подконтрольная Аква-Таррис.", 5);
            factions["megatransen"] = new Faction("megatransen", "MegaTransen", "Инженерно-торговая корпорация.", 0);
            factions["crimson_voyagers"] = new Faction("crimson_voyagers", "Багровые Вояжеры", "Пираты.", -40);
            factions["void_dragons"] = new Faction("void_dragons", "Пустотные Драконы", "Пираты.", -40);

            foreach (var kv in factions)
            {
                reputation[kv.Key] = kv.Value.InitialReputation;
            }
        }

        public static Faction GetFaction(string id)
        {
            return factions.ContainsKey(id) ? factions[id] : null;
        }

        public static int GetReputation(string factionId)
        {
            return reputation.ContainsKey(factionId) ? reputation[factionId] : 0;
        }

        public static void ModifyReputation(string factionId, int delta)
        {
            if (!reputation.ContainsKey(factionId)) return;
            reputation[factionId] += delta;
            if (reputation[factionId] > 100) reputation[factionId] = 100;
            if (reputation[factionId] < -100) reputation[factionId] = -100;
        }

        public static void SetReputation(string factionId, int value)
        {
            if (reputation.ContainsKey(factionId))
                reputation[factionId] = value;
        }

        public static ReputationLevel GetReputationLevel(string factionId)
        {
            int rep = GetReputation(factionId);
            if (rep <= -51) return ReputationLevel.Hostile;
            if (rep <= -1) return ReputationLevel.Negative;
            if (rep <= 20) return ReputationLevel.Neutral;
            if (rep <= 50) return ReputationLevel.Friendly;
            return ReputationLevel.Ally;
        }

        public static List<Faction> GetAllFactions()
        {
            return factions.Values.ToList();
        }

        public static Dictionary<string, int> ExportReputation()
        {
            return new Dictionary<string, int>(reputation);
        }

        public static void ImportReputation(Dictionary<string, int> saved)
        {
            foreach (var kv in saved)
            {
                if (reputation.ContainsKey(kv.Key))
                    reputation[kv.Key] = kv.Value;
            }
        }
    }

    public enum ReputationLevel
    {
        Hostile,
        Negative,
        Neutral,
        Friendly,
        Ally
    }
}