using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases;

public class AddNoteToEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    private readonly IRepositoryFactory _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));

    public async Task<Entities.Note> ExecuteAsync(long etudiantId, long ueId, decimal valeur)
    {
        // Validations simples
        if (etudiantId <= 0) throw new ArgumentOutOfRangeException(nameof(etudiantId));
        if (ueId <= 0) throw new ArgumentOutOfRangeException(nameof(ueId));
        if (valeur < 0m || valeur > 20m) throw new InvalidNoteException("La note doit être comprise entre 0 et 20.");

        var etuRepo = _repositoryFactory.EtudiantRepository() ?? throw new ArgumentNullException("EtudiantRepository");
        var ueRepo = _repositoryFactory.UeRepository() ?? throw new ArgumentNullException("UeRepository");
        var parcoursRepo = _repositoryFactory.ParcoursRepository() ?? throw new ArgumentNullException("ParcoursRepository");
        var noteRepo = _repositoryFactory.NoteRepository() ?? throw new ArgumentNullException("NoteRepository");

        // Vérifier existence étudiant
        var etus = await etuRepo.FindByConditionAsync(e => e.Id == etudiantId);
        if (etus == null || !etus.Any()) throw new ArgumentException($"Étudiant {etudiantId} introuvable.");

        // Vérifier existence UE
        var ues = await ueRepo.FindByConditionAsync(u => u.Id == ueId);
        if (ues == null || !ues.Any()) throw new ArgumentException($"UE {ueId} introuvable.");

        // Vérifier qu'il n'y a pas déjà une note pour (étudiant, ue)
        var existingNotes = await noteRepo.FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        if (existingNotes != null && existingNotes.Any())
            throw new DuplicateNoteException($"L'étudiant {etudiantId} a déjà une note pour l'UE {ueId}.");

        // MODIFICATION ICI : Récupérer tous les parcours (condition toujours vraie)
        var tousParcours = await parcoursRepo.FindByConditionAsync(p => p.Id > 0);
        
        // Filtrer les parcours qui contiennent l'UE (en mémoire)
        var parcoursAvecUe = tousParcours
            .Where(p => p.UesEnseignees != null && p.UesEnseignees.Any(u => u.Id == ueId))
            .ToList();
        
        if (!parcoursAvecUe.Any())
            throw new StudentNotInParcoursException($"L'UE {ueId} n'est associée à aucun parcours.");

        // Vérifier qu'au moins un de ces parcours contient l'étudiant
        bool etudiantDansParcours = parcoursAvecUe.Any(p => p.Inscrits != null && p.Inscrits.Any(e => e.Id == etudiantId));
        if (!etudiantDansParcours)
            throw new StudentNotInParcoursException($"L'étudiant {etudiantId} n'est pas inscrit dans un parcours contenant l'UE {ueId}.");

        // Création de la note
        var note = new Note
        {
            EtudiantId = etudiantId,
            UeId = ueId,
            Valeur = valeur
        };

        return await noteRepo.CreateAsync(note);
    }
}