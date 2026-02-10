using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetTousLesParcoursUseCase(IRepositoryFactory factory)
{
    public async Task<List<Parcours>> ExecuteAsync()
    {
        // On demande au repository de tous les parcours de nous donner la liste complète
        return await factory.ParcoursRepository().FindAllAsync();
    }
}