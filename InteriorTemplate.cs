using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Data
{
    public class InteriorTemplate
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Width { get; set; }
        public int Height { get; set; }
        public List<string> Tiles { get; set; } = new();
        public List<TemplateEntity> Entities { get; set; } = new();
    }

    public class TemplateEntity
    {
        public string Type { get; set; } = "";
        public string Subtype { get; set; } = "";
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public string? DialogueId { get; set; }
        public List<string>? Loot { get; set; }
        public bool Locked { get; set; } = false;
        public string? Description { get; set; }
        public string? Action { get; set; }
        public string? Color { get; set; }
        public string? Symbol { get; set; }
        public string? FactionId { get; set; }
        public List<TraderItemData>? TraderItems { get; set; }
    }

    public class TraderItemData
    {
        public string GoodId { get; set; } = "";
        public int Quantity { get; set; }
        public int PriceModifier { get; set; }
    }
}