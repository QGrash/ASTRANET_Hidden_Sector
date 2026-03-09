using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Data;
using ASTRANET_Hidden_Sector.Screens;
using ASTRANET_Hidden_Sector.World;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace ASTRANET_Hidden_Sector.Entities.Interior
{
    public class InteriorMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private InteriorCell[,] cells;
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public string TemplateId { get; set; } = "";

        public InteriorMap(int width, int height, string templateId = "")
        {
            Width = width;
            Height = height;
            TemplateId = templateId;
            cells = new InteriorCell[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    cells[x, y] = new InteriorCell();
        }

        public InteriorCell? GetCell(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return cells[x, y];
            return null;
        }

        public void SetEntity(int x, int y, MapEntity? entity)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                cells[x, y].Entity = entity;
        }

        public static InteriorMap LoadFromTemplate(InteriorTemplate template)
        {
            var map = new InteriorMap(template.Width, template.Height, template.Id);

            for (int y = 0; y < template.Height; y++)
            {
                if (y >= template.Tiles.Count) break;
                string row = template.Tiles[y];
                for (int x = 0; x < template.Width; x++)
                {
                    if (x >= row.Length) continue;
                    char c = row[x];
                    var cell = map.GetCell(x, y);
                    if (cell == null) continue;

                    switch (c)
                    {
                        case '#':
                            cell.Entity = new WallEntity(x, y);
                            break;
                        case '.':
                            break;
                        case 'C':
                            cell.Entity = new ConsoleEntity("Встроенная консоль", "Мигает огоньками.", "", x, y);
                            break;
                        case 'B':
                            cell.Entity = new StaticWallEntity("Койка", 'B', ConsoleColor.DarkYellow, x, y);
                            break;
                    }
                }
            }

            foreach (var entData in template.Entities)
            {
                int x = entData.X;
                int y = entData.Y;
                if (x < 0 || x >= template.Width || y < 0 || y >= template.Height) continue;

                MapEntity? entity = null;
                switch (entData.Type)
                {
                    case "npc":
                        bool hostile = entData.Subtype?.Contains("hostile") ?? false;
                        var traderItemsData = new List<TraderItemData>();
                        if (entData.TraderItems != null)
                        {
                            foreach (var ti in entData.TraderItems)
                            {
                                traderItemsData.Add(new TraderItemData
                                {
                                    GoodId = ti.GoodId,
                                    Quantity = ti.Quantity,
                                    PriceModifier = ti.PriceModifier
                                });
                            }
                        }
                        entity = new NpcEntity(
                            entData.Name,
                            entData.DialogueId ?? "",
                            hostile,
                            x, y,
                            entData.FactionId ?? "",
                            traderItemsData
                        );
                        break;
                    case "container":
                        entity = new ContainerEntity(entData.Name, entData.Locked, entData.Loot ?? new List<string>(), x, y);
                        break;
                    case "console":
                        entity = new ConsoleEntity(entData.Name, entData.Description ?? "", entData.Action ?? "", x, y);
                        break;
                    case "door":
                        entity = new DoorEntity(entData.Name, entData.Locked, x, y);
                        break;
                }

                if (entity != null)
                {
                    map.SetEntity(x, y, entity);
                }
            }

            map.PlayerX = template.Width / 2;
            map.PlayerY = template.Height / 2;
            return map;
        }

        public SavedInteriorState SaveState()
        {
            var saved = new SavedInteriorState
            {
                TemplateId = this.TemplateId,
                Entities = new List<SavedEntity>()
            };

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cell = cells[x, y];
                    if (cell.Entity != null)
                    {
                        var savedEntity = SaveEntity(cell.Entity);
                        savedEntity.X = x;
                        savedEntity.Y = y;
                        saved.Entities.Add(savedEntity);
                    }
                }
            }
            return saved;
        }

        public void RestoreState(SavedInteriorState saved)
        {
            var template = InteriorLoader.GetTemplate(saved.TemplateId);
            if (template == null) return;

            var newMap = LoadFromTemplate(template);
            this.Width = newMap.Width;
            this.Height = newMap.Height;
            this.cells = newMap.cells;
            this.PlayerX = newMap.PlayerX;
            this.PlayerY = newMap.PlayerY;
            this.TemplateId = newMap.TemplateId;

            foreach (var savedEntity in saved.Entities)
            {
                var cell = GetCell(savedEntity.X, savedEntity.Y);
                if (cell == null) continue;

                MapEntity? entity = CreateEntityFromSaved(savedEntity);
                if (entity != null)
                {
                    cell.Entity = entity;
                }
            }
        }

        private MapEntity? CreateEntityFromSaved(SavedEntity saved)
        {
            switch (saved.Type)
            {
                case "DoorEntity":
                    var door = new DoorEntity(saved.Name, saved.IsLocked, saved.X, saved.Y);
                    door.IsOpen = saved.IsOpen;
                    return door;
                case "ContainerEntity":
                    var container = new ContainerEntity(saved.Name, saved.IsLocked, saved.Loot, saved.X, saved.Y);
                    container.IsOpen = saved.IsOpen;
                    return container;
                case "NpcEntity":
                    return new NpcEntity(saved.Name, saved.DialogueId, false, saved.X, saved.Y);
                case "ConsoleEntity":
                    return new ConsoleEntity(saved.Name, saved.Description, "", saved.X, saved.Y);
                default:
                    return null;
            }
        }

        private SavedEntity SaveEntity(MapEntity entity)
        {
            var saved = new SavedEntity
            {
                Type = entity.GetType().Name,
                Name = entity.Name,
                Description = entity.Description,
                Symbol = entity.Symbol,
                Color = entity.Color,
                X = entity.X,
                Y = entity.Y,
                IsDestroyed = entity.IsDestroyed,
                IsStatic = entity.IsStatic
            };
            if (entity is DoorEntity door)
            {
                saved.IsOpen = door.IsOpen;
                saved.IsLocked = door.IsLocked;
            }
            if (entity is ContainerEntity container)
            {
                saved.IsOpen = container.IsOpen;
                saved.IsLocked = container.IsLocked;
                saved.Loot = container.Loot;
            }
            if (entity is NpcEntity npc)
            {
                saved.DialogueId = npc.DialogueId;
            }
            if (entity is ConsoleEntity console)
            {
                saved.Description = console.Description;
            }
            return saved;
        }

        private class WallEntity : MapEntity
        {
            public WallEntity(int x, int y)
            {
                Name = "Стена";
                Description = "Толстая металлическая стена.";
                Symbol = '#';
                Color = ConsoleColor.Gray;
                X = x;
                Y = y;
                IsSolid = true;
                IsStatic = true;
            }

            public override void Interact(GameStateManager stateManager, UIManager uiManager)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, "Стена", "Это просто стена."));
            }
        }

        private class StaticWallEntity : MapEntity
        {
            public StaticWallEntity(string name, char symbol, ConsoleColor color, int x, int y)
            {
                Name = name;
                Description = "Неинтерактивный объект интерьера.";
                Symbol = symbol;
                Color = color;
                X = x;
                Y = y;
                IsSolid = true;
                IsStatic = true;
            }

            public override void Interact(GameStateManager stateManager, UIManager uiManager)
            {
                stateManager.PushScreen(new MessageScreen(stateManager, uiManager, Name, Description));
            }
        }
    }
}