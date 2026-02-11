using System.Text;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.SaisieEnMasse;

namespace UniversiteDomainUnitTest;

[TestFixture]
public class SaisieEnMasseUnitTest
{
    private Mock<IRepositoryFactory> _mockFactory;
    private Mock<IEtudiantRepository> _mockEtudiantRepo;
    private Mock<IUeRepository> _mockUeRepo;
    private Mock<INoteRepository> _mockNoteRepo;

    [SetUp]
    public void SetUp()
    {
        _mockFactory = new Mock<IRepositoryFactory>();
        _mockEtudiantRepo = new Mock<IEtudiantRepository>();
        _mockUeRepo = new Mock<IUeRepository>();
        _mockNoteRepo = new Mock<INoteRepository>();

        // On lie les mocks à la factory
        _mockFactory.Setup(f => f.EtudiantRepository()).Returns(_mockEtudiantRepo.Object);
        _mockFactory.Setup(f => f.UeRepository()).Returns(_mockUeRepo.Object);
        _mockFactory.Setup(f => f.NoteRepository()).Returns(_mockNoteRepo.Object);
    }

    [Test]
    public async Task Import_NoteSuperieureA20_ShouldThrowExceptionAndSaveNothing()
    {
        // 1. Arrange : Un CSV avec une note de 25/20 (Invalide)
        var csvContent = "NumeroUe;IntituleUe;NumEtud;Nom;Prenom;Note\nUE1;Maths;E1;Dupont;Jean;25";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var useCase = new ImportNotesCsvUseCase(_mockFactory.Object);

        // 2. Act & Assert : On vérifie que le Use Case lance bien une exception
        var ex = Assert.ThrowsAsync<Exception>(async () => await useCase.ExecuteAsync(stream));
        Assert.That(ex.Message, Does.Contain("La note doit être entre 0 et 20"));
        
        // 3. Vérification du "Tout ou Rien"
        // On vérifie que la méthode CreateAsync n'a JAMAIS été appelée
        _mockNoteRepo.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Never);
    }
    
    [Test]
    public async Task Export_ShouldReturnCorrectCsvFormat()
    {
        // 1. Arrange
        var ueId = 1L;
        var etudiant = new Etudiant { Id = 10, NumEtud = "E123", Nom = "Fenty", Prenom = "Anne" };
        var ue = new Ue { Id = ueId, NumeroUe = "GEST01", Intitule = "Gestion" };
    
        // IMPORTANT : On simule la structure que ton UseCase va parcourir
        var parcours = new Parcours { Id = 5 };
        parcours.UesEnseignees = new List<Ue> { ue };
        etudiant.ParcoursSuivi = parcours;

        // On configure les Mocks pour qu'ils répondent aux méthodes exactes du UseCase
        _mockUeRepo.Setup(r => r.FindAsync(ueId)).ReturnsAsync(ue);
        _mockEtudiantRepo.Setup(r => r.FindAllAsync()).ReturnsAsync(new List<Etudiant> { etudiant });
    
        var useCase = new ExportNotesCsvUseCase(_mockFactory.Object);

        // 2. Act
        var result = await useCase.ExecuteAsync(ueId);

        // 3. Assert
        Assert.That(result, Does.Contain("Gestion"));
        Assert.That(result, Does.Contain("Fenty"));
        Assert.That(result, Does.Contain("Anne"));
        Assert.That(result, Does.Contain("E123"));
    }
    
}