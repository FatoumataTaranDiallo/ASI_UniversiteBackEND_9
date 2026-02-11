namespace UniversiteDomain.Dtos;

public class NoteCsvDto
{
    // Informations de l'UE (pour le contexte)
    public string NumeroUe { get; set; } = string.Empty;
    public string IntituleUe { get; set; } = string.Empty;

    // Informations de l'étudiant
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;

    // La note (peut être vide dans le CSV si pas encore saisie)
    // On utilise float? (nullable) pour gérer les cases vides
    public float? Note { get; set; }
}