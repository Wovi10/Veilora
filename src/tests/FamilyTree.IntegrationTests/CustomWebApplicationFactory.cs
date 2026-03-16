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
    public Mock<IPersonService> PersonServiceMock { get; } = new();
    public Mock<ITreeService> TreeServiceMock { get; } = new();
    public Mock<IRelationshipService> RelationshipServiceMock { get; } = new();

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
            RemoveAndMock<IPersonService>(services, PersonServiceMock.Object);
            RemoveAndMock<IRelationshipService>(services, RelationshipServiceMock.Object);
            RemoveAndMock<ITreeService>(services, TreeServiceMock.Object);

            // Remove repository registrations
            RemoveAll<IPersonRepository>(services);
            RemoveAll<IRelationshipRepository>(services);
            RemoveAll<ITreeRepository>(services);

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
