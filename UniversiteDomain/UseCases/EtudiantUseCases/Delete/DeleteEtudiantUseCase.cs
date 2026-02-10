using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long id)
    {
        await factory.EtudiantRepository().DeleteAsync(id);
        await factory.SaveChangesAsync();
    }
    // Seule la scolarité peut supprimer un étudiant
    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite);
}