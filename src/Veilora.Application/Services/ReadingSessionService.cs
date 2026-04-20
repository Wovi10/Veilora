using System.Text.RegularExpressions;
using Veilora.Application.DTOs.ReadingNote;
using Veilora.Application.DTOs.ReadingSession;
using Veilora.Application.Exceptions;
using Veilora.Application.Mappers;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

public partial class ReadingSessionService(
    IReadingSessionRepository sessionRepository,
    IReadingNoteRepository noteRepository,
    IWorldRepository worldRepository
) : IReadingSessionService
{
    [GeneratedRegex(@"#(\w+)")]
    private static partial Regex TagPattern();

    public async Task<ReadingSessionDto> StartAsync(Guid userId, CreateReadingSessionDto dto)
    {
        var world = await worldRepository.GetByIdAsync(dto.WorldId)
            ?? throw new NotFoundException(nameof(World), dto.WorldId);

        var existing = await sessionRepository.GetActiveByUserAsync(userId);
        if (existing is not null)
            throw new BusinessException("End your current session before starting a new one.");

        var session = new ReadingSession
        {
            WorldId = dto.WorldId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
        };

        await sessionRepository.AddAsync(session);
        await sessionRepository.SaveChangesAsync();

        return ReadingSessionMapper.ToDto(session, world.Name, 0);
    }

    public async Task<ReadingSessionDto?> GetActiveAsync(Guid userId)
    {
        var session = await sessionRepository.GetActiveByUserAsync(userId);
        if (session is null) return null;

        var world = await worldRepository.GetByIdAsync(session.WorldId);
        var notes = await noteRepository.GetBySessionAsync(session.Id);

        return ReadingSessionMapper.ToDto(session, world?.Name ?? "Unknown", notes.Count());
    }

    public async Task<IEnumerable<ReadingSessionDto>> GetAllAsync(Guid userId)
    {
        var sessions = await sessionRepository.GetByUserAsync(userId);
        var result = new List<ReadingSessionDto>();

        foreach (var session in sessions)
        {
            var world = await worldRepository.GetByIdAsync(session.WorldId);
            var notes = await noteRepository.GetBySessionAsync(session.Id);
            result.Add(ReadingSessionMapper.ToDto(session, world?.Name ?? "Unknown", notes.Count()));
        }

        return result;
    }

    public async Task EndAsync(Guid sessionId, Guid userId)
    {
        var session = await sessionRepository.GetByIdAsync(sessionId)
            ?? throw new NotFoundException(nameof(ReadingSession), sessionId);

        if (session.UserId != userId)
            throw new BusinessException("Access denied.");

        // GetByIdAsync uses AsNoTracking — re-attach for update
        var tracked = new ReadingSession
        {
            Id = session.Id,
            WorldId = session.WorldId,
            UserId = session.UserId,
            StartedAt = session.StartedAt,
            EndedAt = DateTime.UtcNow,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
        };

        await sessionRepository.UpdateAsync(tracked);
        await sessionRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<ReadingNoteDto>> GetNotesAsync(Guid sessionId, Guid userId)
    {
        var session = await sessionRepository.GetByIdAsync(sessionId)
            ?? throw new NotFoundException(nameof(ReadingSession), sessionId);

        if (session.UserId != userId)
            throw new BusinessException("Access denied.");

        var notes = await noteRepository.GetBySessionAsync(sessionId);
        return notes.Select(ReadingSessionMapper.ToNoteDto);
    }

    public async Task<ReadingNoteDto> AddNoteAsync(Guid sessionId, Guid userId, CreateReadingNoteDto dto)
    {
        var session = await sessionRepository.GetByIdAsync(sessionId)
            ?? throw new NotFoundException(nameof(ReadingSession), sessionId);

        if (session.UserId != userId)
            throw new BusinessException("Access denied.");

        var tags = TagPattern().Matches(dto.Text)
            .Select(m => m.Groups[1].Value.ToLower())
            .Distinct()
            .ToArray();

        var note = new ReadingNote
        {
            SessionId = sessionId,
            WorldId = session.WorldId,
            UserId = userId,
            Text = dto.Text,
            Tags = tags,
        };

        await noteRepository.AddAsync(note);
        await noteRepository.SaveChangesAsync();

        return ReadingSessionMapper.ToNoteDto(note);
    }

    public async Task DeleteNoteAsync(Guid noteId, Guid userId)
    {
        var note = await noteRepository.GetByIdAsync(noteId)
            ?? throw new NotFoundException(nameof(ReadingNote), noteId);

        if (note.UserId != userId)
            throw new BusinessException("Access denied.");

        await noteRepository.DeleteAsync(note);
        await noteRepository.SaveChangesAsync();
    }
}
