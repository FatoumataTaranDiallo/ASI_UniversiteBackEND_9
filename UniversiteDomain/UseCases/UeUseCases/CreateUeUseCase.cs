using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases;

// On change ici : on injecte la Factory (IRepositoryFactory)
public class CreateUeUseCase(IRepositoryFactory factory)
{
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentException.ThrowIfNullOrWhiteSpace(ue.NumeroUe);
        ArgumentException.ThrowIfNullOrWhiteSpace(ue.Intitule);

        if (ue.Intitule.Length <= 3)
            throw new InvalidUeException("L’intitulé de l’UE doit contenir plus de 3 caractères.");

        // On récupère le repository via la factory
        var repository = factory.UeRepository();

        var existing = await repository.FindByConditionAsync(x => x.NumeroUe == ue.NumeroUe);
        if (existing != null && existing.Count > 0)
            throw new DuplicateUeException($"Une UE avec le numéro {ue.NumeroUe} existe déjà.");

        Ue creee = await repository.CreateAsync(ue);
        // On n'oublie pas de sauvegarder via la factory
        await factory.SaveChangesAsync();
        return creee;
    }
}