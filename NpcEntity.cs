using System.Collections.Generic;
using System.Linq;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Data;
using ASTRANET_Hidden_Sector.Entities.Trade;
using ASTRANET_Hidden_Sector.Screens;
using ASTRANET_Hidden_Sector.Combat;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public class NpcEntity : InteriorEntity, ITrader
    {
        public string DialogueId { get; set; } = "";
        public bool IsHostile { get; set; }
        public string FactionId { get; set; } = "";
        public StationInventory? TraderInventory { get; set; }

        // Боевые характеристики
        public GroundCombatant CombatStats { get; set; }

        public List<TraderItem> Items
        {
            get
            {
                if (TraderInventory == null) return new List<TraderItem>();
                return TraderInventory.Items.Select(ii => new TraderItem
                {
                    GoodId = ii.GoodId,
                    Quantity = ii.Quantity,
                    PriceModifier = ii.PriceModifier
                }).ToList();
            }
        }

        public NpcEntity(string name, string dialogueId, bool hostile, int x, int y, string factionId = "", List<TraderItemData>? traderItems = null, GroundCombatant? combatStats = null)
        {
            Name = name;
            DialogueId = dialogueId;
            IsHostile = hostile;
            X = x;
            Y = y;
            IsSolid = true;
            Symbol = 'T';
            Color = hostile ? ConsoleColor.Red : ConsoleColor.Green;
            FactionId = factionId;

            if (traderItems != null && traderItems.Count > 0)
            {
                TraderInventory = new StationInventory();
                TraderInventory.FactionId = factionId;
                foreach (var itemData in traderItems)
                {
                    TraderInventory.Items.Add(new StationInventory.InventoryItem
                    {
                        GoodId = itemData.GoodId,
                        Quantity = itemData.Quantity,
                        PriceModifier = itemData.PriceModifier
                    });
                }
            }

            // Если переданы боевые характеристики, используем их, иначе создаём простые заглушки
            if (combatStats != null)
            {
                CombatStats = combatStats;
            }
            else
            {
                // Заглушка: 50 HP, 30 энергии, 10 брони, 5 уклонения, простой лазер
                var defaultWeapon = new Weapon("Лазерный пистолет", DamageType.Laser, 5, 8, 5, 5, 1, 5, 1.5f, WeaponClass.Light, WeaponMode.Single);
                CombatStats = new GroundCombatant(name, 50, 30, 10, 5, defaultWeapon);
            }
        }

        public int GetBuyPrice(TradeGood good, int reputation)
        {
            if (TraderInventory != null)
                return TraderInventory.GetBuyPrice(good, reputation);
            return good.BasePrice;
        }

        public int GetSellPrice(TradeGood good, int reputation)
        {
            if (TraderInventory != null)
                return TraderInventory.GetSellPrice(good, reputation);
            return good.BasePrice / 2;
        }

        public override void Interact(GameStateManager stateManager, UIManager uiManager)
        {
            if (IsHostile)
            {
                // Вместо простого сообщения теперь начинаем бой
                // Мы вызовем метод, который передаст управление в боевой режим
                StartCombat(stateManager, uiManager);
            }
            else
            {
                if (!string.IsNullOrEmpty(DialogueId))
                {
                    stateManager.PushScreen(new DialogueScreen(stateManager, uiManager, DialogueId, this));
                }
                else
                {
                    stateManager.PushScreen(new MessageScreen(stateManager, uiManager, Name,
                        $"{Name} смотрит на вас и молчит."));
                }
            }
        }

        private void StartCombat(GameStateManager stateManager, UIManager uiManager)
        {
            // Здесь нужно будет вызвать специальный экран или режим для наземного боя
            // Пока оставим заглушку
            stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Бой",
                $"Начинается бой с {Name}!"));
        }
    }
}