using FamilyTree.Application.DTOs.Tree;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;

namespace FamilyTree.Application.Services;

public class TreeService : ITreeService
{
    private readonly ITreeRepository _treeRepository;
    private readonly IPersonRepository _personRepository;

    public TreeService(ITreeRepository treeRepository, IPersonRepository personRepository)
    {
        _treeRepository = treeRepository;
        _personRepository = personRepository;
    }

    public async Task<IEnumerable<TreeDto>> GetAllAsync()
    {
        var trees = await _treeRepository.GetAllAsync();
        return trees.Select(TreeMapper.ToDto);
    }

    public async Task<TreeDto?> GetByIdAsync(Guid id)
    {
        var tree = await _treeRepository.GetByIdAsync(id);
        if (tree == null)
        {
            throw new NotFoundException($"Tree with ID {id} not found");
        }

        return TreeMapper.ToDto(tree);
    }

    public async Task<TreeWithPersonsDto?> GetTreeWithPersonsAsync(Guid id)
    {
        var tree = await _treeRepository.GetTreeWithPersonsAsync(id);
        if (tree == null)
        {
            throw new NotFoundException($"Tree with ID {id} not found");
        }

        return TreeMapper.ToTreeWithPersonsDto(tree);
    }

    public async Task<TreeDto> CreateAsync(CreateTreeDto dto)
    {
        var tree = TreeMapper.ToEntity(dto);
        await _treeRepository.AddAsync(tree);
        await _treeRepository.SaveChangesAsync();

        return TreeMapper.ToDto(tree);
    }

    public async Task<TreeDto> UpdateAsync(Guid id, UpdateTreeDto dto)
    {
        var tree = await _treeRepository.GetByIdAsync(id);
        if (tree == null)
        {
            throw new NotFoundException($"Tree with ID {id} not found");
        }

        TreeMapper.UpdateEntity(dto, tree);
        await _treeRepository.UpdateAsync(tree);
        await _treeRepository.SaveChangesAsync();

        return TreeMapper.ToDto(tree);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tree = await _treeRepository.GetByIdAsync(id);
        if (tree == null)
        {
            throw new NotFoundException($"Tree with ID {id} not found");
        }

        await _treeRepository.DeleteAsync(tree);
        await _treeRepository.SaveChangesAsync();
    }

    public async Task AddPersonToTreeAsync(Guid treeId, Guid personId)
    {
        var tree = await _treeRepository.GetByIdAsync(treeId);
        if (tree == null)
        {
            throw new NotFoundException($"Tree with ID {treeId} not found");
        }

        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {personId} not found");
        }

        // Check if person is already in tree
        var isInTree = await _treeRepository.IsPersonInTreeAsync(treeId, personId);
        if (isInTree)
        {
            throw new BusinessException($"Person with ID {personId} is already in tree {treeId}");
        }

        await _treeRepository.AddPersonToTreeAsync(treeId, personId);
        await _treeRepository.SaveChangesAsync();
    }

    public async Task RemovePersonFromTreeAsync(Guid treeId, Guid personId)
    {
        var tree = await _treeRepository.GetByIdAsync(treeId);
        if (tree == null)
        {
            throw new NotFoundException($"Tree with ID {treeId} not found");
        }

        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {personId} not found");
        }

        // Check if person is in tree
        var isInTree = await _treeRepository.IsPersonInTreeAsync(treeId, personId);
        if (!isInTree)
        {
            throw new BusinessException($"Person with ID {personId} is not in tree {treeId}");
        }

        await _treeRepository.RemovePersonFromTreeAsync(treeId, personId);
        await _treeRepository.SaveChangesAsync();
    }

    public async Task UpdatePersonPositionAsync(Guid treeId, Guid personId, double x, double y)
    {
        var isInTree = await _treeRepository.IsPersonInTreeAsync(treeId, personId);
        if (!isInTree)
        {
            throw new NotFoundException($"Person with ID {personId} not found in tree {treeId}");
        }

        await _treeRepository.UpdatePersonPositionAsync(treeId, personId, x, y);
    }
}

