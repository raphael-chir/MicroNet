using System.Text.Json.Serialization;

namespace MicroNet.Domain
{
    public class Contrat
    {
        public string Id { get; set; }
        public string NumSocietaire { get; set; }
        public DateTime DatePremierEffet { get; set; }
        public List<SituationRef> SituationsRefs { get; set; }

        public Contrat(){
            
        }

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