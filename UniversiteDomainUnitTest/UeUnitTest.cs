using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.UeUseCases;
using static UniversiteDomain.UseCases.UeUseCases.CreateUeUseCase;

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

        // Pas de doublon
        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>());

        // Retour de création
        mock.Setup(repo => repo.CreateAsync(ueSansId)).ReturnsAsync(ueCree);

        var useCase = new CreateUeUseCase(mock.Object);
        var result = await useCase.ExecuteAsync(ueSansId);

        Assert.That(result.Id, Is.EqualTo(ueCree.Id));
        Assert.That(result.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
        Assert.That(result.Intitule, Is.EqualTo(ueCree.Intitule));
    }

    [Test]
    public void CreateUeUseCase_InvalidIntitule_ThrowsException()
    {
        var mock = new Mock<IUeRepository>();
        var useCase = new CreateUeUseCase(mock.Object);
        var ueInvalide = new Ue { NumeroUe = "UE002", Intitule = "AB" };

        Assert.ThrowsAsync<InvalidUeException>(async () => await useCase.ExecuteAsync(ueInvalide));
    }
}