using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        // On vérifie d'abord si un utilisateur est bien lié à cet étudiant
        var user = await factory.UniversiteUserRepository().FindByEtudiantIdAsync(idEtudiant);
        
        if (user != null)
        {
            // On utilise l'ID de l'étudiant pour supprimer le compte utilisateur lié
            await factory.UniversiteUserRepository().DeleteAsync(idEtudiant);
            await factory.SaveChangesAsync();
        }
    }
    
    // Seule la scolarité est autorisée à supprimer
    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite);
}