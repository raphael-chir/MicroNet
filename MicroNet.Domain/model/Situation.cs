namespace MicroNet.Domain
{
    public class Situation
    {
        public string Id { get; private set; }
        public string Operation { get; private set; }
        public DateTime DatePremierEffet { get; private set; }
        public List<Assure> Assures { get; private set; }
        public List<Resiliation> Resiliations { get; private set; }
        public List<OffrePromotionnelle> OffrePromotionnelles { get; private set; }

        public Situation(string id, string operation, DateTime datePremierEffet, List<Assure> assures, List<Resiliation> resiliations, List<OffrePromotionnelle> offrePromotionnelles)
        {
            Id = id;
            Operation = operation;
            DatePremierEffet = datePremierEffet;
            Assures = assures;
            Resiliations = resiliations;
            OffrePromotionnelles = offrePromotionnelles;
        }
    }
}