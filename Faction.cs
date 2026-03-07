namespace ASTRANET_Hidden_Sector.Entities.Faction
{
    public class Faction
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int InitialReputation { get; set; } = 0;

        public Faction() { }

        public Faction(string id, string name, string description, int initialRep = 0)
        {
            Id = id;
            Name = name;
            Description = description;
            InitialReputation = initialRep;
        }
    }
}