using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.UseCases.UeUseCases;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UeController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<UeDto>> PostAsync([FromBody] UeDto ueDto)
        {
            var uc = new CreateUeUseCase(repositoryFactory);
            var ue = await uc.ExecuteAsync(ueDto.ToEntity());
            return Ok(new UeDto().ToDto(ue));
        }
    }
}