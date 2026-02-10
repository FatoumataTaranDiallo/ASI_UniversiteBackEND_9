using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create;

// On change ici : on injecte la Factory (IRepositoryFactory) au lieu du repo seul
public class CreateEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant> ExecuteAsync(string numEtud, string nom, string prenom, string email)
    {
        var etudiant = new Etudiant{NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email};
        return await ExecuteAsync(etudiant);
    }
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        // On utilise la factory pour obtenir le repo
        Etudiant et = await factory.EtudiantRepository().CreateAsync(etudiant);
        // On utilise la factory pour sauvegarder de manière asynchrone (plus propre que .Wait())
        await factory.SaveChangesAsync();
        return et;
    }
    public bool IsAuthorized(string role)
    {
        // Seuls les responsables et la scolarité peuvent créer un étudiant
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(factory);
        
        // On récupère le repository via la factory pour les vérifications métier
        var etudiantRepository = factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
        
        // On recherche un étudiant avec le même numéro étudiant
        List<Etudiant> existe = await etudiantRepository.FindByConditionAsync(e=>e.NumEtud.Equals(etudiant.NumEtud));

        // Si un étudiant avec le même numéro étudiant existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNumEtudException(etudiant.NumEtud+ " - ce numéro d'étudiant est déjà affecté à un étudiant");
        
        // Vérification du format du mail
        if (!CheckEmail.IsValidEmail(etudiant.Email)) throw new InvalidEmailException(etudiant.Email + " - Email mal formé");
        
        // On vérifie si l'email est déjà utilisé
        existe = await etudiantRepository.FindByConditionAsync(e=>e.Email.Equals(etudiant.Email));
        // Une autre façon de tester la vacuité de la liste
        if (existe is {Count:>0}) throw new DuplicateEmailException(etudiant.Email +" est déjà affecté à un étudiant");
        // Le métier définit que les nom doite contenir plus de 3 lettres
        if (etudiant.Nom.Length < 3) throw new InvalidNomEtudiantException(etudiant.Nom +" incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }
}