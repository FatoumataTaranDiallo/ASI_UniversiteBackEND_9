using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentException.ThrowIfNullOrWhiteSpace(parcours.NomParcours);

        // Vérifier que le parcours n’existe pas déjà (même nom et même année)
        var existing = await repositoryFactory
            .ParcoursRepository()
            .FindByConditionAsync(p => p.NomParcours == parcours.NomParcours 
                                       && p.AnneeFormation == parcours.AnneeFormation);

        if (existing != null && existing.Count > 0)
            throw new DuplicateParcoursException(
                $"Le parcours '{parcours.NomParcours}' en année {parcours.AnneeFormation} existe déjà."
            );

        // Création en base
        return await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
    }
}