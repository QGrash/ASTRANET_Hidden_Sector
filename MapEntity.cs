using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.World
{
    public abstract class MapEntity
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public char Symbol { get; set; } = '·';
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsVisible { get; set; } = false;
        public bool IsExplored { get; set; } = false;
        public bool IsDestroyed { get; set; } = false;
        public bool IsStatic { get; set; } = true; // по умолчанию true для статических объектов

        public abstract void Interact(GameStateManager stateManager, UIManager uiManager);
    }
}