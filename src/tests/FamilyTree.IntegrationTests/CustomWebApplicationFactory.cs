using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FamilyTree.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IEntityService> EntityServiceMock { get; } = new();
    public Mock<IFamilyTreeService> FamilyTreeServiceMock { get; } = new();
    public Mock<IRelationshipService> RelationshipServiceMock { get; } = new();
    public Mock<IWorldService> WorldServiceMock { get; } = new();
    public Mock<INoteService> NoteServiceMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove DbContext (requires Postgres)
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbDescriptor is not null) services.Remove(dbDescriptor);

            // Replace all services that pull in repositories/DbContext
            RemoveAndMock<IEntityService>(services, EntityServiceMock.Object);
            RemoveAndMock<IFamilyTreeService>(services, FamilyTreeServiceMock.Object);
            RemoveAndMock<IRelationshipService>(services, RelationshipServiceMock.Object);
            RemoveAndMock<IWorldService>(services, WorldServiceMock.Object);
            RemoveAndMock<INoteService>(services, NoteServiceMock.Object);

            // Remove repository registrations
            RemoveAll<IEntityRepository>(services);
            RemoveAll<IFamilyTreeRepository>(services);
            RemoveAll<IRelationshipRepository>(services);
            RemoveAll<IWorldRepository>(services);
            RemoveAll<INoteRepository>(services);

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
