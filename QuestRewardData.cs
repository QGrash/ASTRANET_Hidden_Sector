namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public class QuestRewardData
    {
        public RewardType Type { get; set; }
        public string Target { get; set; } = ""; // ID предмета, фракции
        public int Amount { get; set; } // количество или значение
        public bool IsOptional { get; set; } = false; // для выбора награды
    }

    public enum RewardType
    {
        Credits,
        Item,
        Reputation,
        Experience,
        TechPoints,
        UnlockModule,
        UnlockQuest
    }
}