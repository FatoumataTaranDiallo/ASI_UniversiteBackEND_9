using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly UniversiteDbContext _context;

    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;

    public RepositoryFactory(UniversiteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IParcoursRepository ParcoursRepository()
        => _parcours ??= new ParcoursRepository(_context);

    public IEtudiantRepository EtudiantRepository()
        => _etudiants ??= new EtudiantRepository(_context);

    public IUeRepository UeRepository()
        => _ues ??= new UeRepository(_context);

    public INoteRepository NoteRepository()
        => _notes ??= new NoteRepository(_context);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public async Task EnsureCreatedAsync()
        => await _context.Database.EnsureCreatedAsync();

    public async Task EnsureDeletedAsync()
        => await _context.Database.EnsureDeletedAsync();
}