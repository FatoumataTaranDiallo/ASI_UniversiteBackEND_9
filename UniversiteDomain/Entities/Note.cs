namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    // Valeur de la note (0 à 20) : decimal pour précision
    public decimal Valeur { get; set; }

    // Navigation properties (optionnel -> utiles pour tests / infra)
    public Etudiant? Etudiant { get; set; }
    public Ue? Ue { get; set; }

    public override string ToString()
    {
        return $"Note(Id={Id}, EtudiantId={EtudiantId}, UeId={UeId}, Valeur={Valeur})";
    }
}
