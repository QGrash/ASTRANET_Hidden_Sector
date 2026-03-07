using System.Collections.Generic;

namespace ASTRANET_Hidden_Sector.Entities
{
    public class Player
    {
        public string Name { get; set; } = "";
        public Background? Background { get; set; }
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int Credits { get; set; } = 1000; // стартовые кредиты

        // Характеристики
        public int Strength { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Luck { get; set; } = 10;
        public int Charisma { get; set; } = 10;

        // Инвентарь
        public Inventory Inventory { get; set; }

        // Экипировка (пока заглушка)
        public Dictionary<string, Item?> Equipment { get; set; } = new Dictionary<string, Item?>();

        public Player(string name, Background bg)
        {
            Name = name;
            Background = bg;

            // Применяем бонусы предыстории
            if (bg.StatBonuses != null)
            {
                if (bg.StatBonuses.ContainsKey("Strength")) Strength += bg.StatBonuses["Strength"];
                if (bg.StatBonuses.ContainsKey("Dexterity")) Dexterity += bg.StatBonuses["Dexterity"];
                if (bg.StatBonuses.ContainsKey("Intelligence")) Intelligence += bg.StatBonuses["Intelligence"];
                if (bg.StatBonuses.ContainsKey("Luck")) Luck += bg.StatBonuses["Luck"];
                if (bg.StatBonuses.ContainsKey("Charisma")) Charisma += bg.StatBonuses["Charisma"];
            }

            // Грузоподъёмность = 20 + Strength * 5
            float maxWeight = 20 + Strength * 5;
            Inventory = new Inventory(maxWeight);
        }
    }
}