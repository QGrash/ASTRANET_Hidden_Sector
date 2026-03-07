namespace ASTRANET_Hidden_Sector.World
{
    public class LocalMapCell
    {
        public MapEntity? Entity { get; set; }   // теперь здесь может быть любой объект (статический или динамический)
        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }
        public char Symbol { get; set; } = '.';
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;
    }
}