namespace MicroNet.Domain
{
    public class Contrat
    {
        public string Id { get; private set; }
        public string Type { get; private set; }

        public Contrat(string id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}