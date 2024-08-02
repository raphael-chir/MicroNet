namespace MicroNet.Domain
{
    public class OffrePromotionnelle
    {
        public DateTime DateDebutEffet { get; private set; }
        public string Code { get; private set; }
        public OffrePromotionnelle(DateTime dateDebutEffet, string code)
        {
            DateDebutEffet = dateDebutEffet;
            Code = code;
        } 
    }
}