using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Entities.Trade;

namespace ASTRANET_Hidden_Sector.Entities
{
    public class StationInventory
    {
        public string FactionId { get; set; } = "";
        public List<InventoryItem> Items { get; set; } = new();

        public class InventoryItem
        {
            public string GoodId { get; set; } = "";
            public int Quantity { get; set; }
            public int PriceModifier { get; set; }
        }

        public int GetBuyPrice(TradeGood good, int reputation)
        {
            float repModifier = GetReputationModifier(reputation);
            return (int)(good.BasePrice * repModifier);
        }

        public int GetSellPrice(TradeGood good, int reputation)
        {
            float repModifier = GetReputationModifier(reputation);
            return (int)(good.BasePrice * 0.5f * repModifier);
        }

        private float GetReputationModifier(int reputation)
        {
            if (reputation >= 51) return 0.8f;
            if (reputation >= 21) return 0.9f;
            if (reputation >= 0) return 1.0f;
            if (reputation >= -50) return 1.3f;
            return 2.0f;
        }
    }
}