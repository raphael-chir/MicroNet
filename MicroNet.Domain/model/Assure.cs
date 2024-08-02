namespace MicroNet.Domain
{
    public class Assure
    {
        public string Nom { get; private set; }
        public string Prenom { get; private set; }
        
        public Assure(string nom, string prenom)
        {
            Nom = nom;
            Prenom = prenom;
        }
    }
}