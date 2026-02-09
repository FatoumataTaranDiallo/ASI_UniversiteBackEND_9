using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // On garde uniquement cette version du GET pour l'instant
        [HttpGet("{id}", Name = "GetUnEtudiant")]
        public async Task<ActionResult<EtudiantDto>> GetUnEtudiant(long id)
        {
            // On simule une attente pour enlever le warning
            return await Task.FromResult(Ok());
        }

        // Ton POST corrigé avec la gestion des erreurs (Étape 6)
        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            // Correction importante : on passe le repo extrait de la factory
            var repo = repositoryFactory.EtudiantRepository();
            CreateEtudiantUseCase createEtudiantUc = new CreateEtudiantUseCase(repo);           
            
            Etudiant etud = etudiantDto.ToEntity();
            try
            {
                etud = await createEtudiantUc.ExecuteAsync(etud);
            }
            catch (Exception e)
            {
                // Gestion des erreurs métier (doublons, etc.) -> Code HTTP 400
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            EtudiantDto dto = new EtudiantDto().ToDto(etud);
            // Retourne un code HTTP 201 (Created)
            return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.Id }, dto);
        }
    }
}