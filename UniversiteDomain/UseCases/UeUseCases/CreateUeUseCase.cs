using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases;

public class CreateUeUseCase(IUeRepository repository)
{
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentException.ThrowIfNullOrWhiteSpace(ue.NumeroUe);
        ArgumentException.ThrowIfNullOrWhiteSpace(ue.Intitule);

        if (ue.Intitule.Length <= 3)
            throw new InvalidUeException("L’intitulé de l’UE doit contenir plus de 3 caractères.");

        var existing = await repository.FindByConditionAsync(x => x.NumeroUe == ue.NumeroUe);
        if (existing != null && existing.Count > 0)
            throw new DuplicateUeException($"Une UE avec le numéro {ue.NumeroUe} existe déjà.");

        return await repository.CreateAsync(ue);
    }
}