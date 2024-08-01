using MicroNet.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroNet.Application
{
    public class ContratService
    {
        private readonly IContratRepository _contratRepository;

        public ContratService(IContratRepository contratRepository)
        {
            _contratRepository = contratRepository;
        }

        public async Task<IEnumerable<Contrat>> GetAllContratsAsync()
        {
            return await _contratRepository.GetAllAsync();
        }

        public async Task<Contrat> GetContratByIdAsync(int id)
        {
            return await _contratRepository.GetByIdAsync(id);
        }
    }
}
