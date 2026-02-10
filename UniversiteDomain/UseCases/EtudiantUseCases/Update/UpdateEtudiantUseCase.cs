using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        await factory.EtudiantRepository().UpdateAsync(etudiant);
        await factory.SaveChangesAsync();
    }
    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite);
}