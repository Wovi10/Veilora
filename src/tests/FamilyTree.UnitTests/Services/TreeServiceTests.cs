using FamilyTree.Application.DTOs.FamilyTree;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services;
using FamilyTree.Domain.Entities;
using FluentAssertions;
using Moq;
using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.UnitTests.Services;

[TestFixture]
public class FamilyTreeServiceTests
{
    private Mock<IFamilyTreeRepository> _familyTreeRepositoryMock;
    private Mock<IEntityRepository> _entityRepositoryMock;
    private FamilyTreeService _sut;

    [SetUp]
    public void SetUp()
    {
        _familyTreeRepositoryMock = new Mock<IFamilyTreeRepository>();
        _entityRepositoryMock = new Mock<IEntityRepository>();
        _sut = new FamilyTreeService(_familyTreeRepositoryMock.Object, _entityRepositoryMock.Object);
    }

    private static FamilyTreeEntity MakeFamilyTree(Guid id, DateTime now) => new()
    {
        Id = id,
        Name = "Smith Family",
        WorldId = Guid.NewGuid(),
        CreatedAt = now,
        UpdatedAt = now
    };

    [Test]
    public async Task GetAllAsync_WhenTreesExist_ReturnsMappedDtos()
    {
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var trees = new List<FamilyTreeEntity>
        {
            MakeFamilyTree(id1, now),
            MakeFamilyTree(id2, now)
        };
        _familyTreeRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(trees);

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result.Select(t => t.Id).Should().Contain([id1, id2]);
    }

    [Test]
    public async Task GetAllAsync_WhenNoTreesExist_ReturnsEmptyCollection()
    {
        _familyTreeRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetByIdAsync_WhenTreeExists_ReturnsMappedDto()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = MakeFamilyTree(treeId, now);
        tree.Description = "Desc";
        _familyTreeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);

        var result = await _sut.GetByIdAsync(treeId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(treeId);
        result.Name.Should().Be("Smith Family");
        result.Description.Should().Be("Desc");
    }

    [Test]
    public async Task GetByIdAsync_WhenTreeDoesNotExist_ReturnsNull()
    {
        var treeId = Guid.NewGuid();
        _familyTreeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((FamilyTreeEntity?)null);

        var result = await _sut.GetByIdAsync(treeId);

        result.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_CreatesTreeAndReturnsMappedDto()
    {
        var worldId = Guid.NewGuid();
        var dto = new CreateFamilyTreeDto { Name = "New Tree", Description = "A new tree", WorldId = worldId };
        _familyTreeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<FamilyTreeEntity>())).ReturnsAsync((FamilyTreeEntity t) => t);
        _familyTreeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Name.Should().Be("New Tree");
        result.Description.Should().Be("A new tree");
        _familyTreeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FamilyTreeEntity>()), Times.Once);
        _familyTreeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenTreeExists_UpdatesAndReturnsMappedDto()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = MakeFamilyTree(treeId, now);
        var dto = new UpdateFamilyTreeDto { Name = "New Name", Description = "Updated" };
        _familyTreeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _familyTreeRepositoryMock.Setup(r => r.UpdateAsync(tree)).Returns(Task.CompletedTask);
        _familyTreeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateAsync(treeId, dto);

        result.Name.Should().Be("New Name");
        result.Description.Should().Be("Updated");
        _familyTreeRepositoryMock.Verify(r => r.UpdateAsync(tree), Times.Once);
        _familyTreeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenTreeDoesNotExist_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        _familyTreeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((FamilyTreeEntity?)null);

        var act = async () => await _sut.UpdateAsync(treeId, new UpdateFamilyTreeDto { Name = "X" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DeleteAsync_WhenTreeExists_DeletesAndSaves()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = MakeFamilyTree(treeId, now);
        _familyTreeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _familyTreeRepositoryMock.Setup(r => r.DeleteAsync(tree)).Returns(Task.CompletedTask);
        _familyTreeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(treeId);

        _familyTreeRepositoryMock.Verify(r => r.DeleteAsync(tree), Times.Once);
        _familyTreeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenTreeDoesNotExist_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        _familyTreeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((FamilyTreeEntity?)null);

        var act = async () => await _sut.DeleteAsync(treeId);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
