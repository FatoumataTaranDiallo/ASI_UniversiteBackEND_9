using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory; // Ajouté pour la factory
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.UeUseCases;

namespace UniversiteDomainUnitTest;

public class UeUnitTest
{
    [SetUp]
    public void Setup() {}

    [Test]
    public async Task CreateUeUseCase_Success()
    {
        string numero = "UE001";
        string intitule = "Programmation C#";

        var ueSansId = new Ue { NumeroUe = numero, Intitule = intitule };
        var ueCree = new Ue { Id = 1, NumeroUe = numero, Intitule = intitule };

        var mock = new Mock<IUeRepository>();
        
        // --- AJOUT POUR LA FACTORY ---
        var mockFactory = new Mock<IRepositoryFactory>();
        // On dit à la factory de renvoyer notre mock de repo UE
        mockFactory.Setup(f => f.UeRepository()).Returns(mock.Object);
        // -----------------------------

        // Pas de doublon
        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>());

        // Retour de création
        mock.Setup(repo => repo.CreateAsync(ueSansId)).ReturnsAsync(ueCree);

        // On passe mockFactory.Object au lieu de mock.Object
        var useCase = new CreateUeUseCase(mockFactory.Object);
        var result = await useCase.ExecuteAsync(ueSansId);

        Assert.That(result.Id, Is.EqualTo(ueCree.Id));
        Assert.That(result.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
        Assert.That(result.Intitule, Is.EqualTo(ueCree.Intitule));
    }

    [Test]
    public void CreateUeUseCase_InvalidIntitule_ThrowsException()
    {
        var mock = new Mock<IUeRepository>();
        
        // --- AJOUT POUR LA FACTORY ---
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mock.Object);
        // -----------------------------
        
        var useCase = new CreateUeUseCase(mockFactory.Object);
        var ueInvalide = new Ue { NumeroUe = "UE002", Intitule = "AB" };

        Assert.ThrowsAsync<InvalidUeException>(async () => await useCase.ExecuteAsync(ueInvalide));
    }
}