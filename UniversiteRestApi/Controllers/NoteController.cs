using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.UseCases.NoteUseCases;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NoteDto>> PostAsync([FromBody] NoteDto noteDto)
    {
        // On utilise le Use Case pour ajouter la note
        var uc = new AddNoteToEtudiantUseCase(repositoryFactory);
        
        // On exécute avec les données du DTO
        var note = await uc.ExecuteAsync(noteDto.EtudiantId, noteDto.UeId, noteDto.Valeur);
        
        // On renvoie le DTO créé
        return Ok(new NoteDto().ToDto(note));
    }
}