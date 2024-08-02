using MicroNet.Domain;
using Couchbase.Extensions.DependencyInjection;
using System.Text;
using Couchbase.Core.IO.Transcoders;

namespace MicroNet.Infrastructure
{
    public class ContratRepository : IContratRepository
    {
        private readonly INamedBucketProvider _provider;

        public ContratRepository(INamedBucketProvider provider)
        {
            _provider = provider;
        }

        public Task<IEnumerable<Contrat>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Contrat> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
