namespace MicroNet.Domain
{
    public class SituationRef
    {
        public string Id { get; private set; }
        public DateTime DatePremierEffet { get; private set; }
            public SituationRef(string id, DateTime datePremierEffet)
        {
            Id = id;
            DatePremierEffet = datePremierEffet;
        }
       
    }
}