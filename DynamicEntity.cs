namespace ASTRANET_Hidden_Sector.World
{
    public abstract class DynamicEntity : MapEntity
    {
        public bool IsMoving { get; set; } = false;
        public int Speed { get; set; } = 1;
        public int TargetX { get; set; }
        public int TargetY { get; set; }

        public abstract void UpdateTurn(LocalMap map);
    }
}