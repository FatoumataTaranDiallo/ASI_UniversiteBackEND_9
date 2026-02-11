using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.UseCases.ParcoursUseCases;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;


namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ParcoursDto>>> GetAsync()
        {
            // Pas de CheckSecu ici pour simplifier, ou ajoute-le si tu veux
            var uc = new GetTousLesParcoursUseCase(repositoryFactory);
            var parcours = await uc.ExecuteAsync();
            return ParcoursDto.ToDtos(parcours);
        }

        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> PostAsync([FromBody] ParcoursDto parcoursDto)
        {
            var uc = new CreateParcoursUseCase(repositoryFactory);
            var p = await uc.ExecuteAsync(parcoursDto.ToEntity());
            return CreatedAtAction(nameof(GetAsync), new { id = p.Id }, new ParcoursDto().ToDto(p));
        }
    }
}