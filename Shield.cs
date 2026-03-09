namespace ASTRANET_Hidden_Sector.Combat
{
    public class Shield
    {
        public int Current { get; set; }
        public int Max { get; set; }
        public int RegenRate { get; set; }

        public Shield(int max, int regenRate = 0)
        {
            Max = max;
            Current = max;
            RegenRate = regenRate;
        }

        public int AbsorbDamage(int incomingDamage)
        {
            int absorbed = System.Math.Min(Current, incomingDamage);
            Current -= absorbed;
            return incomingDamage - absorbed;
        }

        public void Regenerate()
        {
            Current = System.Math.Min(Max, Current + RegenRate);
        }

        public bool IsActive => Current > 0;
    }
}