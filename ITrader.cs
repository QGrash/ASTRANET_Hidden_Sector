using ASTRANET_Hidden_Sector.Entities.Trade;
using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities
{
    public interface ITrader
    {
        string FactionId { get; }
        List<TraderItem> Items { get; }
        int GetBuyPrice(TradeGood good, int reputation);
        int GetSellPrice(TradeGood good, int reputation);
    }

    public class TraderItem
    {
        public string GoodId { get; set; } = "";
        public int Quantity { get; set; }
        public int PriceModifier { get; set; }
    }
}