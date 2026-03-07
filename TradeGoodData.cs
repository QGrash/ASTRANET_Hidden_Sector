using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities.Trade
{
    // Класс для загрузки из JSON
    public class TradeGoodData
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = ""; // строка, потом преобразуем в GoodCategory
        public float Weight { get; set; }
        public int BasePrice { get; set; }
        public bool IsLegal { get; set; } = true;
        public int HealAmount { get; set; } = 0;
        public string ModuleId { get; set; } = ""; // для модулей
    }
}