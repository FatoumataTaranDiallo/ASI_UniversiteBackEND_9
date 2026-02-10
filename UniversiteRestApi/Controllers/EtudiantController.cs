using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.EtudiantUseCases.Delete;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.EtudiantUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Delete;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteDomain.UseCases.SecurityUseCases.Update;
using UniversiteEFDataProvider.Entities;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<EtudiantDto>>> GetAsync()
        {
            string role=""; string email=""; IUniversiteUser user=null;
            try { CheckSecu(out role, out email, out user); } catch { return Unauthorized(); }
            
            // On passe repositoryFactory directement (pas .EtudiantRepository())
            GetTousLesEtudiantsUseCase uc = new GetTousLesEtudiantsUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            
            try {
                var etud = await uc.ExecuteAsync();
                return EtudiantDto.ToDtos(etud);
            } catch { return ValidationProblem(); }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EtudiantDto>> GetUnEtudiant(long id)
        {
            string role=""; string email=""; IUniversiteUser user = null;
            try { CheckSecu(out role, out email, out user); } catch { return Unauthorized(); }
            
            GetEtudiantByIdUseCase uc = new GetEtudiantByIdUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role, user, id)) return Unauthorized();
            
            try {
                var etud = await uc.ExecuteAsync(id);
                if (etud == null) return NotFound();
                return new EtudiantDto().ToDto(etud);
            } catch (Exception e) {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        [HttpGet("complet/{id}")]
        public async Task<ActionResult<EtudiantCompletDto>> GetUnEtudiantCompletAsync(long id)
        {
            string role=""; string email=""; IUniversiteUser user = null;
            try { CheckSecu(out role, out email, out user); } catch { return Unauthorized(); }
            
            GetEtudiantCompletUseCase uc = new GetEtudiantCompletUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role, user, id)) return Unauthorized();
            
            try {
                var etud = await uc.ExecuteAsync(id);
                if (etud == null) return NotFound();
                return new EtudiantCompletDto().ToDto(etud);
            } catch { return ValidationProblem(); }
        }

        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            CreateEtudiantUseCase createEtudiantUc = new CreateEtudiantUseCase(repositoryFactory);
            CreateUniversiteUserUseCase createUserUc = new CreateUniversiteUserUseCase(repositoryFactory);

            string role=""; string email=""; IUniversiteUser user = null;
            CheckSecu(out role, out email, out user);
            if (!createEtudiantUc.IsAuthorized(role) || !createUserUc.IsAuthorized(role)) return Unauthorized();
            
            Etudiant etud = etudiantDto.ToEntity();
            try { etud = await createEtudiantUc.ExecuteAsync(etud); }
            catch (Exception e) {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            try {
                await createUserUc.ExecuteAsync(etud.Email, etud.Email, "Miage2025#", Roles.Etudiant, etud); 
            } catch (Exception e) {
                await new DeleteEtudiantUseCase(repositoryFactory).ExecuteAsync(etud.Id);
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            EtudiantDto dto = new EtudiantDto().ToDto(etud);
            return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EtudiantDto>> PutAsync(long id, [FromBody] EtudiantDto etudiantDto)
        {
            if (id != etudiantDto.Id) return BadRequest();

            UpdateEtudiantUseCase updateEtudiantUc = new UpdateEtudiantUseCase(repositoryFactory);
            UpdateUniversiteUserUseCase updateUserUc = new UpdateUniversiteUserUseCase(repositoryFactory);

            string role=""; string email=""; IUniversiteUser user = null;
            try { CheckSecu(out role, out email, out user); } catch { return Unauthorized(); }
            
            if (!updateEtudiantUc.IsAuthorized(role)|| !updateUserUc.IsAuthorized(role)) return Unauthorized();
            
            try {
                await updateUserUc.ExecuteAsync(etudiantDto.ToEntity());
                await updateEtudiantUc.ExecuteAsync(etudiantDto.ToEntity());
                return NoContent();
            } catch (Exception e) {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(long id)
        {
            DeleteEtudiantUseCase etudiantUc = new DeleteEtudiantUseCase(repositoryFactory);
            DeleteUniversiteUserUseCase userUc = new DeleteUniversiteUserUseCase(repositoryFactory);
            
            string role = ""; string email = ""; IUniversiteUser user = null;
            try { CheckSecu(out role, out email, out user); } catch { return Unauthorized(); }

            if (!etudiantUc.IsAuthorized(role) || !userUc.IsAuthorized(role)) return Unauthorized();
            
            try {
                await userUc.ExecuteAsync(id);
                await etudiantUc.ExecuteAsync(id);
                return NoContent();
            } catch (Exception e) {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = ""; email = "";
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.FindFirst(ClaimTypes.Email) == null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email).Value;
            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user == null || claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null || claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role).Value;
        }
    }
}