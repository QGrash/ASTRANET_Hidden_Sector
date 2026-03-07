namespace ASTRANET_Hidden_Sector.Entities
{
    public enum ItemType
    {
        Consumable,  // расходники (медпакеты, еда)
        Equipment,   // экипировка (броня, оружие)
        Quest,       // квестовые предметы
        Resource,    // ресурсы (топливо, металл)
        Module,      // модули корабля
        Misc         // прочие предметы (добавлено для восстановления и общих целей)
    }

    public class Item
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public ItemType Type { get; set; }
        public float Weight { get; set; }
        public int Value { get; set; }
        public int HealAmount { get; set; }

        public Item() { }

        public Item(string id, string name, string desc, ItemType type, float weight, int value)
        {
            Id = id;
            Name = name;
            Description = desc;
            Type = type;
            Weight = weight;
            Value = value;
        }
    }
}