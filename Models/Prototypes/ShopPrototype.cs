// Models/Prototypes/ShopPrototype.cs
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ASTRANET.Models.Prototypes;

public class ShopPrototype
{
    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Faction")]
    public FactionId? Faction { get; set; } // Опционально: к какой фракции принадлежит магазин

    [JsonProperty("Items")]
    public List<ShopItem> Items { get; set; } = new();
}

public class ShopItem
{
    [JsonProperty("ItemId")]
    public string ItemId { get; set; }

    [JsonProperty("PriceModifier")]
    public double PriceModifier { get; set; } = 1.0;

    [JsonProperty("Quantity")]
    public int Quantity { get; set; } = -1;
}