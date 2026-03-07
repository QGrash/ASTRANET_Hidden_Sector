using System.Collections.Generic;
using System.Linq;

namespace ASTRANET_Hidden_Sector.Entities
{
    public class Inventory
    {
        private List<Item> items = new List<Item>();
        public float MaxWeight { get; set; }
        public float CurrentWeight => items.Sum(i => i.Weight);

        public Inventory(float maxWeight)
        {
            MaxWeight = maxWeight;
        }

        public bool AddItem(Item item)
        {
            if (CurrentWeight + item.Weight <= MaxWeight)
            {
                items.Add(item);
                return true;
            }
            return false; // перегруз
        }

        public bool RemoveItem(Item item)
        {
            return items.Remove(item);
        }

        public List<Item> GetAllItems()
        {
            return new List<Item>(items);
        }

        public Item? FindItem(string id)
        {
            return items.FirstOrDefault(i => i.Id == id);
        }

        public List<Item> GetItemsByType(ItemType type)
        {
            return items.Where(i => i.Type == type).ToList();
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}