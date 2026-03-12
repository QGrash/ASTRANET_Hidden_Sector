// Models/Instances/ItemInstance.cs
using ASTRANET.Models.Prototypes;

namespace ASTRANET.Models.Instances;

public class ItemInstance
{
    public string PrototypeId { get; set; }
    public int Quantity { get; set; } = 1;
    public int CurrentDurability { get; set; } = -1;

    [Newtonsoft.Json.JsonIgnore]
    public ItemPrototype Prototype { get; set; }

    public ItemInstance() { }

    public ItemInstance(string prototypeId, int quantity = 1)
    {
        PrototypeId = prototypeId;
        Quantity = quantity;
    }
}