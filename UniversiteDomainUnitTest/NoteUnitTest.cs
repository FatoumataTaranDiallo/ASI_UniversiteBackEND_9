using Moq;
using System.Linq.Expressions;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases;

namespace UniversiteDomainUnitTest;

public class NoteUnitTest
{
    [SetUp] public void Setup() {}

    [Test]
    public async Task AddNote_Success()
    {
        // Arrange
        long etuId = 1, ueId = 10;
        decimal valeur = 15.5m;

        var mockEtu = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        var mockNote = new Mock<INoteRepository>();

        // fake etudiant
        mockEtu.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { new Etudiant { Id = etuId } });

        mockUe.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { new Ue { Id = ueId } });

        var parcours = new Parcours { Id = 2, UesEnseignees = new List<Ue> { new Ue { Id = ueId } }, Inscrits = new List<Etudiant> { new Etudiant { Id = etuId } } };
        mockParcours.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        mockNote.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>());

        // fake create
        var created = new Note { Id = 5, EtudiantId = etuId, UeId = ueId, Valeur = valeur };
        mockNote.Setup(r => r.CreateAsync(It.IsAny<Note>())).ReturnsAsync(created);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtu.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcours.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNote.Object);

        var useCase = new AddNoteToEtudiantUseCase(mockFactory.Object);

        // Act
        var res = await useCase.ExecuteAsync(etuId, ueId, valeur);

        // Assert
        Assert.That(res, Is.Not.Null);
        Assert.That(res.Id, Is.EqualTo(created.Id));
        mockNote.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Once);
    }

    [Test]
    public void AddNote_Duplicate_Throws()
    {
        // Arrange similar but mockNote.FindByConditionAsync returns a list with a note -> expect DuplicateNoteException
    }

    // Ajoute d'autres tests : note hors limite, étudiant pas inscrit -> exceptions
}
