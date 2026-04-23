using Veilora.Api.Services;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Veilora.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IEntityService> EntityServiceMock { get; } = new();
    public Mock<IFamilyTreeService> FamilyTreeServiceMock { get; } = new();
    public Mock<IRelationshipService> RelationshipServiceMock { get; } = new();
    public Mock<IWorldService> WorldServiceMock { get; } = new();
    public Mock<INoteService> NoteServiceMock { get; } = new();
    public Mock<ICharacterService> CharacterServiceMock { get; } = new();
    public Mock<IReadingSessionService> ReadingSessionServiceMock { get; } = new();
    public Mock<IEventService> EventServiceMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove DbContext and all EF options (requires Postgres)
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            // Replace tested services with mocks; strip all others that depend on repositories
            RemoveAndMock<IEntityService>(services, EntityServiceMock.Object);
            RemoveAndMock<IFamilyTreeService>(services, FamilyTreeServiceMock.Object);
            RemoveAndMock<IRelationshipService>(services, RelationshipServiceMock.Object);
            RemoveAndMock<IWorldService>(services, WorldServiceMock.Object);
            RemoveAndMock<INoteService>(services, NoteServiceMock.Object);

            RemoveAndMock<ICharacterService>(services, CharacterServiceMock.Object);
            RemoveAll<ILocationService>(services);
            RemoveAll<IAuthService>(services);
            RemoveAll<ITokenService>(services);
            RemoveAll<IWorldPermissionService>(services);
            RemoveAll<ILanguageService>(services);
            RemoveAll<IDateSuffixService>(services);
            RemoveAndMock<IReadingSessionService>(services, ReadingSessionServiceMock.Object);
            RemoveAndMock<IEventService>(services, EventServiceMock.Object);
            RemoveAll<ISearchService>(services);

            // Remove all repository registrations
            RemoveAll<IEntityRepository>(services);
            RemoveAll<IFamilyTreeRepository>(services);
            RemoveAll<IRelationshipRepository>(services);
            RemoveAll<IWorldRepository>(services);
            RemoveAll<INoteRepository>(services);
            RemoveAll<ICharacterRepository>(services);
            RemoveAll<ILocationRepository>(services);
            RemoveAll<IUserRepository>(services);
            RemoveAll<IWorldPermissionRepository>(services);
            RemoveAll<ILanguageRepository>(services);
            RemoveAll<IDateSuffixRepository>(services);
            RemoveAll<IReadingSessionRepository>(services);
            RemoveAll<IReadingNoteRepository>(services);
            RemoveAll<IEventRepository>(services);

            // Replace JWT auth with a test scheme that always authenticates
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    private static void RemoveAndMock<TService>(IServiceCollection services, TService instance)
        where TService : class
    {
        var d = services.SingleOrDefault(x => x.ServiceType == typeof(TService));
        if (d is not null) services.Remove(d);
        services.AddScoped<TService>(_ => instance);
    }

    private static void RemoveAll<TService>(IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(TService)).ToList();
        foreach (var d in descriptors) services.Remove(d);
    }
}
