using Microsoft.AspNetCore.Mvc;
using MicroNet.Application;
using MicroNet.Domain;

namespace MicroNet.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContratsController : ControllerBase
    {
        private readonly ContratService _contratService;

        public ContratsController(ContratService contratService)
        {
            _contratService = contratService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contrat>>> GetAll()
        {
            var contrats = await _contratService.GetAllContratsAsync();
            return Ok(contrats);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contrat>> GetById(int id)
        {
            var contrat = await _contratService.GetContratByIdAsync(id);
            if (contrat == null)
                return NotFound();
            return Ok(contrat);
        }

    }
}
