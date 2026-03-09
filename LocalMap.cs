using System;

namespace ASTRANET_Hidden_Sector.World
{
    public class LocalMap
    {
        public const int Width = 20;
        public const int Height = 20;

        private LocalMapCell[,] cells;

        public int PlayerX { get; set; }
        public int PlayerY { get; set; }

        public LocalMap()
        {
            cells = new LocalMapCell[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    cells[x, y] = new LocalMapCell();
        }

        public LocalMap(Data.SavedLocalMap saved)
        {
            cells = new LocalMapCell[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    cells[x, y] = new LocalMapCell();

            foreach (var cellData in saved.Cells)
            {
                var cell = GetCell(cellData.X, cellData.Y);
                if (cell != null)
                {
                    cell.IsVisible = cellData.IsVisible;
                    cell.IsExplored = cellData.IsExplored;
                    cell.Symbol = cellData.Symbol;
                    cell.Color = cellData.Color;
                    if (cellData.Entity != null)
                    {
                        cell.Entity = new StaticEntity(
                            new Location
                            {
                                Name = cellData.Entity.Name,
                                Description = cellData.Entity.Description,
                                Type = ParseLocationType(cellData.Entity.LocationType),
                                X = cellData.Entity.X,
                                Y = cellData.Entity.Y
                            },
                            cellData.Entity.DialogueId
                        );
                    }
                }
            }

            PlayerX = Width / 2;
            PlayerY = Height / 2;
        }

        private LocationType ParseLocationType(string typeStr)
        {
            return typeStr switch
            {
                "Planet" => LocationType.Planet,
                "Station" => LocationType.Station,
                "Enemy" => LocationType.Enemy,
                "Anomaly" => LocationType.Anomaly,
                "Resources" => LocationType.Resources,
                "Hidden" => LocationType.Hidden,
                _ => LocationType.Empty
            };
        }

        public LocalMapCell? GetCell(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return cells[x, y];
            return null;
        }

        public void SetCell(int x, int y, MapEntity entity)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                cells[x, y].Entity = entity;
                UpdateCellAppearance(x, y);
            }
        }

        public void UpdateCellAppearance(int x, int y)
        {
            var cell = cells[x, y];
            if (cell.Entity != null && !cell.Entity.IsDestroyed)
            {
                cell.Symbol = cell.Entity.Symbol;
                cell.Color = cell.Entity.Color;
            }
            else
            {
                cell.Symbol = '·';
                cell.Color = ConsoleColor.DarkGray;
            }
        }

        public void UpdateVisibility(int sightRange = 5)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    int dx = Math.Abs(x - PlayerX);
                    int dy = Math.Abs(y - PlayerY);
                    bool inSight = dx <= sightRange && dy <= sightRange;
                    cells[x, y].IsVisible = inSight;
                    if (inSight)
                        cells[x, y].IsExplored = true;
                }
        }

        public void UpdateAllEntities()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    if (cells[x, y].Entity is DynamicEntity dyn)
                        dyn.UpdateTurn(this);
                }
        }
    }
}