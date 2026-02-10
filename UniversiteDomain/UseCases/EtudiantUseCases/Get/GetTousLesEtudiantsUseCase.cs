using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetTousLesEtudiantsUseCase(IRepositoryFactory factory)
{
    public async Task<List<Etudiant>> ExecuteAsync()
    {
        return await factory.EtudiantRepository().FindAllAsync();
    }
    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
}