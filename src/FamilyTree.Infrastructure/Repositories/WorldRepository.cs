using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;

namespace FamilyTree.Infrastructure.Repositories;

public class WorldRepository(ApplicationDbContext context) : Repository<World>(context), IWorldRepository
{
}
