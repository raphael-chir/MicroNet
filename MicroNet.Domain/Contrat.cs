namespace MicroNet.Domain
{
    public class Contrat
    {
        public int Id { get; private set; }
        public string Type { get; private set; }

        public Contrat(int id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}