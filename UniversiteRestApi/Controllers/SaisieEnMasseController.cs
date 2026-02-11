using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.UseCases.NoteUseCases.SaisieEnMasse;

namespace UniversiteRestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// SEULE la scolarité peut accéder à ce contrôleur
[Authorize(Roles = "Scolarite,Responsable")] 
public class SaisieEnMasseController : ControllerBase
{
    // 1. Exportation : Télécharger le modèle CSV
    [HttpGet("export/{ueId}")]
    public async Task<IActionResult> ExportNotes(long ueId, [FromServices] ExportNotesCsvUseCase useCase)
    {
        try
        {
            var csvContent = await useCase.ExecuteAsync(ueId);
            // On transforme la string en fichier téléchargeable
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(bytes, "text/csv", $"Notes_UE_{ueId}.csv");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 2. Importation : Envoyer le fichier CSV rempli
    [HttpPost("import")]
    public async Task<IActionResult> ImportNotes(IFormFile file, [FromServices] ImportNotesCsvUseCase useCase)
    {
        if (file == null || file.Length == 0) return BadRequest("Fichier vide ou invalide");

        try
        {
            // On ouvre le flux du fichier envoyé et on le passe au Use Case
            using var stream = file.OpenReadStream();
            await useCase.ExecuteAsync(stream);
            return Ok(new { message = "Toutes les notes ont été importées avec succès !" });
        }
        catch (Exception ex)
        {
            // Si une erreur survient (note > 20, etc.), on renvoie l'erreur
            return BadRequest(new { error = ex.Message });
        }
    }
}