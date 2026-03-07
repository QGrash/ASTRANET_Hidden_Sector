namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public class QuestObjectiveData
    {
        public string Id { get; set; } = "";
        public string Description { get; set; } = "";
        public ObjectiveType Type { get; set; }
        public string Target { get; set; } = ""; // ID предмета, врага, локации
        public int RequiredAmount { get; set; } = 1;
        public string RelatedLocation { get; set; } = ""; // для задач "посетить место"
    }

    public enum ObjectiveType
    {
        CollectItem,     // собрать предметы
        KillEnemy,       // убить врагов
        TalkToNpc,       // поговорить с NPC
        VisitLocation,   // посетить локацию
        DeliverItem,     // передать предмет
        UseItem,         // использовать предмет
        ReachSector,     // достичь сектора
        ReachSystem      // достичь системы
    }
}