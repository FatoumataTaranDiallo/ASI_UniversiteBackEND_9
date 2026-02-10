using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        // On récupère luser lié à l'étudiant pour mettre à jour son email/login
        var user = await factory.UniversiteUserRepository().FindByEtudiantIdAsync(etudiant.Id);
        if (user != null)
        {
            await factory.UniversiteUserRepository().UpdateAsync(user, etudiant.Email, etudiant.Email);
            await factory.SaveChangesAsync();
        }
    }
    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite);
}