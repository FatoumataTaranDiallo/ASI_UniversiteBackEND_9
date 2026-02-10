using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Repositories;
using UniversiteEFDataProvider.Entities; // Important pour UniversiteUser et UniversiteRole

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly UniversiteDbContext _context;
    private readonly UserManager<UniversiteUser> _userManager;
    private readonly RoleManager<UniversiteRole> _roleManager;

    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    // On n'utilise pas de cache (?) ici car ils sont créés à la volée avec les Managers
  
    // MISE À JOUR DU CONSTRUCTEUR
    public RepositoryFactory(UniversiteDbContext context, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IParcoursRepository ParcoursRepository()
        => _parcours ??= new ParcoursRepository(_context);

    public IEtudiantRepository EtudiantRepository()
        => _etudiants ??= new EtudiantRepository(_context);

    public IUeRepository UeRepository()
        => _ues ??= new UeRepository(_context);

    public INoteRepository NoteRepository()
        => _notes ??= new NoteRepository(_context);

    // ON REMPLACE LES THROW PAR LA CRÉATION RÉELLE
    public IUniversiteRoleRepository UniversiteRoleRepository()
        => new UniversiteRoleRepository(_context, _roleManager);

    public IUniversiteUserRepository UniversiteUserRepository()
        => new UniversiteUserRepository(_context, _userManager, _roleManager);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public async Task EnsureCreatedAsync()
        => await _context.Database.EnsureCreatedAsync();

    public async Task EnsureDeletedAsync()
        => await _context.Database.EnsureDeletedAsync();
}