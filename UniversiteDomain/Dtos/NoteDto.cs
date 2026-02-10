using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteDto
{
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    public decimal Valeur { get; set; }

    // Transforme l'entité Note en DTO
    public NoteDto ToDto(Note note)
    {
        this.EtudiantId = note.EtudiantId;
        this.UeId = note.UeId;
        this.Valeur = note.Valeur;
        return this;
    }

    // Si tu as besoin de transformer le DTO en entité Note
    public Note ToEntity()
    {
        return new Note 
        { 
            EtudiantId = this.EtudiantId, 
            UeId = this.UeId, 
            Valeur = this.Valeur 
        };
    }
}