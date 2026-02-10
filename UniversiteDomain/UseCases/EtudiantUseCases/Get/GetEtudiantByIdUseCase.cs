using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant?> ExecuteAsync(long id)
    {
        return await factory.EtudiantRepository().FindAsync(id);
    }
    public bool IsAuthorized(string role, IUniversiteUser user, long idEtudiant)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        return role.Equals(Roles.Etudiant) && user.Etudiant?.Id == idEtudiant;
    }
}