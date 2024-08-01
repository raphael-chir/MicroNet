using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroNet.Domain
{
    public interface IContratRepository
    {
        Task<IEnumerable<Contrat>> GetAllAsync();
        Task<Contrat> GetByIdAsync(int id);
    }
}
