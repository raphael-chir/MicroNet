using MicroNet.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroNet.Infrastructure
{
    public class ContratRepository : IContratRepository
    {
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
