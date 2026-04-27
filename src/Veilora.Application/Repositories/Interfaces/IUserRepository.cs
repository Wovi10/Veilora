using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByDisplayNameAsync(string displayName);
    Task<User?> GetWithBackupAsync(Guid userId);
}
