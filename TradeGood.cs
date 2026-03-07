namespace ASTRANET_Hidden_Sector.Entities.Trade
{
    public enum GoodCategory
    {
        Consumable,
        Equipment,
        Weapon,
        Module,
        Resource,
        Luxury,
        Illegal,
        Quest
    }

    public class TradeGood
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public GoodCategory Category { get; set; }
        public float Weight { get; set; }
        public int BasePrice { get; set; }
        public bool IsLegal { get; set; } = true;
        public int HealAmount { get; set; } = 0;
        public string ModuleId { get; set; } = "";

        public TradeGood() { }

        public TradeGood(string id, string name, string desc, GoodCategory cat, float weight, int basePrice)
        {
            Id = id;
            Name = name;
            Description = desc;
            Category = cat;
            Weight = weight;
            BasePrice = basePrice;
        }

        // Конструктор из данных (исправлено: принимает TradeGoodData)
        public TradeGood(TradeGoodData data)
        {
            Id = data.Id;
            Name = data.Name;
            Description = data.Description;
            Weight = data.Weight;
            BasePrice = data.BasePrice;
            IsLegal = data.IsLegal;
            HealAmount = data.HealAmount;
            ModuleId = data.ModuleId;

            // Преобразуем строку в enum
            Category = data.Category switch
            {
                "Consumable" => GoodCategory.Consumable,
                "Equipment" => GoodCategory.Equipment,
                "Weapon" => GoodCategory.Weapon,
                "Module" => GoodCategory.Module,
                "Resource" => GoodCategory.Resource,
                "Luxury" => GoodCategory.Luxury,
                "Illegal" => GoodCategory.Illegal,
                "Quest" => GoodCategory.Quest,
                _ => GoodCategory.Resource
            };
        }
    }
}