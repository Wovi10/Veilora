using Veilora.Application.DTOs.World;
using Veilora.Application.Exceptions;
using Veilora.Application.Mappers;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

public class WorldService(IWorldRepository worldRepository, IUserRepository userRepository) : IWorldService
{
    public async Task<IEnumerable<WorldDto>> GetAllByUserAsync(Guid userId)
    {
        var worlds = await worldRepository.GetAllByUserAsync(userId);
        return worlds.Select(WorldMapper.ToDto);
    }

    public async Task<WorldDto?> GetByIdAsync(Guid id)
    {
        var world = await worldRepository.GetByIdAsync(id);
        return world is null ? null : WorldMapper.ToDto(world);
    }

    public async Task<WorldDto> CreateAsync(CreateWorldDto dto, Guid? createdById = null)
    {
        var world = WorldMapper.ToEntity(dto, createdById);
        await worldRepository.AddAsync(world);
        await worldRepository.SaveChangesAsync();
        return WorldMapper.ToDto(world);
    }

    public async Task<WorldDto> UpdateAsync(Guid id, UpdateWorldDto dto)
    {
        var world = await worldRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(World), id);
        WorldMapper.UpdateEntity(dto, world);
        await worldRepository.UpdateAsync(world);
        await worldRepository.SaveChangesAsync();
        return WorldMapper.ToDto(world);
    }

    public async Task DeleteAsync(Guid id)
    {
        var world = await worldRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(World), id);
        await worldRepository.DeleteAsync(world);
        await worldRepository.SaveChangesAsync();
    }

    public async Task<bool> TransferOwnershipAsync(Guid id, string newOwnerEmail)
    {
        var newOwner = await userRepository.GetByEmailAsync(newOwnerEmail);
        if (newOwner is null) return false;

        var world = await worldRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(World), id);
        world.CreatedById = newOwner.Id;
        await worldRepository.UpdateAsync(world);
        await worldRepository.SaveChangesAsync();
        return true;
    }
}
