namespace ASTRANET_Hidden_Sector.Entities.Quest
{
    public class QuestReward
    {
        public RewardType Type { get; set; }
        public string Target { get; set; }
        public int Amount { get; set; }

        public QuestReward(QuestRewardData data)
        {
            Type = data.Type;
            Target = data.Target;
            Amount = data.Amount;
        }

        public void Grant()
        {
            // Здесь будет логика выдачи награды
        }
    }
}