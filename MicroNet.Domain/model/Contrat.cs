namespace MicroNet.Domain
{
    public class Contrat
    {
        public string Id { get; private set; }
        public string NumSocietaire { get; private set; }
        public DateTime DatePremierEffet { get; set; }
        public List<SituationRef> SituationsRefs { get; private set; }

        public Contrat(string id, string numSocietaire, DateTime datePremierEffet, List<SituationRef> situationsRefs)
        {
            Id = id;
            NumSocietaire = numSocietaire;
            DatePremierEffet = datePremierEffet;
            SituationsRefs = situationsRefs;
        }

         public Contrat(string id, string numSocietaire, DateTime datePremierEffet)
        {
            Id = id;
            NumSocietaire = numSocietaire;
            DatePremierEffet = datePremierEffet;
            SituationsRefs = new List<SituationRef>();
        }

    }
}