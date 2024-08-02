namespace MicroNet.Domain
{
    public class Resiliation
    {
        public DateTime DateReceptionDemande { get; private set; }
        public string Motif { get; private set; }
        
        public Resiliation(DateTime dateReceptionDemande, string motif)
        {
            DateReceptionDemande = dateReceptionDemande;
            Motif = motif;
        }
    }
}