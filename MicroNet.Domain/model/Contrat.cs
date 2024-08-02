namespace MicroNet.Domain
{
    public class Contrat
    {
        public string Id { get; private set; }
        public string NumSocietaire { get; private set; }
        public DateTime DatePremierEffet { get; private set; }
        public List<SituationRef> Situations { get; private set; }

        public Contrat(string id, string numSocietaire, DateTime datePremierEffet, List<SituationRef> situations)
        {
            Id = id;
            NumSocietaire = numSocietaire;
            DatePremierEffet = datePremierEffet;
            Situations = situations;
        }

    }
}