// Models/Instances/Inventory.cs
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Models.Prototypes;

namespace ASTRANET.Models.Instances;

public class Inventory
{
    private List<ItemInstance> _items = new();

    public IReadOnlyList<ItemInstance> Items => _items.AsReadOnly();

    public bool AddItem(ItemInstance item)
    {
        if (item == null) return false;

        // Проверяем, что прототип существует
        if (item.Prototype == null)
        {
            // Можно попытаться загрузить прототип через ItemManager, но для простоты просто не добавляем
            return false;
        }

        // Только для не-экипировки и не-модулей проверяем возможность стака
        if (item.Prototype.ItemType != ItemType.Equipment && item.Prototype.ItemType != ItemType.Module)
        {
            var existing = _items.FirstOrDefault(i => i.PrototypeId == item.PrototypeId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                return true;
            }
        }
        _items.Add(item);
        return true;
    }

    public bool RemoveItem(ItemInstance item, int quantity = 1)
    {
        if (item == null) return false;
        var existing = _items.FirstOrDefault(i => i == item);
        if (existing == null) return false;

        if (existing.Quantity > quantity)
        {
            existing.Quantity -= quantity;
            return true;
        }
        else if (existing.Quantity == quantity)
        {
            _items.Remove(existing);
            return true;
        }
        return false;
    }

    public bool HasItem(string prototypeId, int quantity = 1)
    {
        var item = _items.FirstOrDefault(i => i.PrototypeId == prototypeId);
        return item != null && item.Quantity >= quantity;
    }

    public double GetTotalWeight()
    {
        double total = 0;
        foreach (var item in _items)
        {
            if (item.Prototype != null)
                total += item.Prototype.Weight * item.Quantity;
        }
        return total;
    }
}