using Veilora.Application.DTOs.Photo;
using Veilora.Application.Exceptions;
using Veilora.Application.Mappers;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;

namespace Veilora.Application.Services;

public class PhotoService(
    IPhotoRepository photoRepository,
    ICharacterRepository characterRepository,
    ILocationRepository locationRepository,
    IEntityRepository entityRepository,
    IWorldRepository worldRepository,
    IWorldPermissionRepository worldPermissionRepository) : IPhotoService
{
    public async Task<IEnumerable<PhotoDto>> GetByEntityAsync(PhotoTargetType type, Guid entityId)
    {
        await ResolveWorldIdAsync(type, entityId);
        var photos = await photoRepository.GetByEntityAsync(type, entityId);
        return photos.Select(PhotoMapper.ToDto);
    }

    public async Task<PhotoDto> AddAsync(PhotoTargetType type, Guid entityId, Guid requestingUserId, CreatePhotoDto dto)
    {
        var worldId = await ResolveWorldIdAsync(type, entityId);
        await AssertCanEditAsync(worldId, requestingUserId);

        var count = await photoRepository.CountByEntityAsync(type, entityId);
        if (count >= 5)
            throw new BusinessException("Maximum of 5 photos per entity.");

        ValidateUrl(dto.Url);

        var maxOrder = await photoRepository.GetMaxDisplayOrderAsync(type, entityId);
        var displayOrder = maxOrder + 1;

        var photo = PhotoMapper.ToEntity(dto, type, entityId, displayOrder);
        await photoRepository.AddAsync(photo);
        await photoRepository.SaveChangesAsync();

        return PhotoMapper.ToDto(photo);
    }

    public async Task<PhotoDto> UpdateAsync(Guid photoId, Guid requestingUserId, UpdatePhotoDto dto)
    {
        var photo = await photoRepository.GetByIdAsync(photoId)
            ?? throw new NotFoundException(nameof(Photo), photoId);

        var worldId = await ResolveWorldIdAsync(photo.EntityType, photo.EntityId);
        await AssertCanEditAsync(worldId, requestingUserId);

        ValidateUrl(dto.Url);

        photo.Url = dto.Url;
        await photoRepository.UpdateAsync(photo);
        await photoRepository.SaveChangesAsync();

        return PhotoMapper.ToDto(photo);
    }

    public async Task DeleteAsync(Guid photoId, Guid requestingUserId)
    {
        var photo = await photoRepository.GetByIdAsync(photoId)
            ?? throw new NotFoundException(nameof(Photo), photoId);

        var worldId = await ResolveWorldIdAsync(photo.EntityType, photo.EntityId);
        await AssertCanEditAsync(worldId, requestingUserId);

        await photoRepository.DeleteAsync(photo);
        await photoRepository.SaveChangesAsync();
    }

    public async Task ReorderAsync(PhotoTargetType type, Guid entityId, Guid requestingUserId, ReorderPhotosDto dto)
    {
        var worldId = await ResolveWorldIdAsync(type, entityId);
        await AssertCanEditAsync(worldId, requestingUserId);

        var photos = (await photoRepository.GetByEntityAsync(type, entityId)).ToList();
        var photoIds = photos.Select(p => p.Id).ToHashSet();
        var providedIds = dto.OrderedIds.ToHashSet();

        if (!photoIds.SetEquals(providedIds))
            throw new BusinessException("Provided IDs do not match the photos for this entity.");

        var photoMap = photos.ToDictionary(p => p.Id);
        for (var i = 0; i < dto.OrderedIds.Count; i++)
        {
            var photo = photoMap[dto.OrderedIds[i]];
            photo.DisplayOrder = i;
            await photoRepository.UpdateAsync(photo);
        }

        await photoRepository.SaveChangesAsync();
    }

    private async Task<Guid> ResolveWorldIdAsync(PhotoTargetType type, Guid entityId) =>
        type switch
        {
            PhotoTargetType.Character => (await characterRepository.GetByIdAsync(entityId)
                ?? throw new NotFoundException(nameof(Character), entityId)).WorldId,
            PhotoTargetType.Location => (await locationRepository.GetByIdAsync(entityId)
                ?? throw new NotFoundException(nameof(Location), entityId)).WorldId,
            PhotoTargetType.Entity => (await entityRepository.GetByIdAsync(entityId)
                ?? throw new NotFoundException(nameof(Entity), entityId)).WorldId,
            _ => throw new BusinessException("Unknown entity type.")
        };

    private async Task AssertCanEditAsync(Guid worldId, Guid requestingUserId)
    {
        var world = await worldRepository.GetByIdAsync(worldId)
            ?? throw new NotFoundException(nameof(World), worldId);

        if (world.CreatedById == requestingUserId)
            return;

        var permission = await worldPermissionRepository.GetAsync(worldId, requestingUserId);
        if (permission?.CanEdit == true)
            return;

        throw new BusinessException("You do not have permission to modify photos in this world.");
    }

    private static void ValidateUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ValidationException("Url", "URL is required.");
        if (url.Length > 2000)
            throw new ValidationException("Url", "URL must not exceed 2000 characters.");
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ValidationException("Url", "URL must be a valid absolute URI.");
    }
}
