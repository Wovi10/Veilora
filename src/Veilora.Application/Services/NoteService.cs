using Veilora.Application.DTOs.Note;
using Veilora.Application.Exceptions;
using Veilora.Application.Mappers;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

public class NoteService(INoteRepository noteRepository) : INoteService
{
    public async Task<IEnumerable<NoteDto>> GetAllAsync()
    {
        var notes = await noteRepository.GetAllAsync();
        return notes.Select(NoteMapper.ToDto);
    }

    public async Task<NoteDto?> GetByIdAsync(Guid id)
    {
        var note = await noteRepository.GetByIdAsync(id);
        return note is null ? null : NoteMapper.ToDto(note);
    }

    public async Task<IEnumerable<NoteDto>> GetByWorldIdAsync(Guid worldId)
    {
        var notes = await noteRepository.GetByWorldIdAsync(worldId);
        return notes.Select(NoteMapper.ToDto);
    }

    public async Task<IEnumerable<NoteDto>> GetByEntityIdAsync(Guid entityId)
    {
        var notes = await noteRepository.GetByEntityIdAsync(entityId);
        return notes.Select(NoteMapper.ToDto);
    }

    public async Task<NoteDto> CreateAsync(CreateNoteDto dto)
    {
        var note = NoteMapper.ToEntity(dto);
        await noteRepository.AddAsync(note);
        await noteRepository.SaveChangesAsync();
        return NoteMapper.ToDto(note);
    }

    public async Task<NoteDto> UpdateAsync(Guid id, UpdateNoteDto dto)
    {
        var note = await noteRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Note), id);
        NoteMapper.UpdateEntity(dto, note);
        await noteRepository.UpdateAsync(note);
        await noteRepository.SaveChangesAsync();
        return NoteMapper.ToDto(note);
    }

    public async Task DeleteAsync(Guid id)
    {
        var note = await noteRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Note), id);
        await noteRepository.DeleteAsync(note);
        await noteRepository.SaveChangesAsync();
    }
}
