using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Infrastructure.Persistence;
using ProyectoAtlas.Infrastructure.Projects;

namespace ProyectoAtlas.Api.Tests;

public class ApiTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private const string TestConnectionString =
      "Host=localhost;Port=5432;Database=atlas_test;Username=atlas;Password=atlas_dev_password";

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Development");

    builder.ConfigureServices(services =>
    {
      services.RemoveAll<DbContextOptions<ProyectoAtlasDbContext>>();
      services.RemoveAll<IProjectRepository>();

      services.AddDbContext<ProyectoAtlasDbContext>(options =>
              options.UseNpgsql(TestConnectionString));

      services.AddScoped<IProjectRepository>(serviceProvider =>
      {
        ProjectRepository innerRepository = ActivatorUtilities.CreateInstance<ProjectRepository>(serviceProvider);
        return new ThrowingProjectRepository(innerRepository);
      });
    });
  }

  public async Task InitializeAsync()
  {
    using IServiceScope scope = Services.CreateScope();
    ProyectoAtlasDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProyectoAtlasDbContext>();

    await dbContext.Database.MigrateAsync();
  }

  Task IAsyncLifetime.DisposeAsync()
  {
    return Task.CompletedTask;
  }

  public async Task ResetDatabaseAsync()
  {
    using IServiceScope scope = Services.CreateScope();
    ProyectoAtlasDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProyectoAtlasDbContext>();

    await dbContext.Database.ExecuteSqlRawAsync(
        """
        TRUNCATE TABLE documentations, projects RESTART IDENTITY CASCADE;
        """);
  }

  public async Task SeedProjectsAsync()
  {
    using IServiceScope scope = Services.CreateScope();
    ProyectoAtlasDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProyectoAtlasDbContext>();

    Project[] projects =
    [
            new Project(
                "Proyecto Atlas",
                "Backend for project documentation based on markdown",
                "https://github.com/matigaleanodev/proyecto-atlas-api",
                "#1E293B"),
            new Project(
                "Atlas Docs",
                "Documentation explorer and reader",
                "https://github.com/matigaleanodev/atlas-docs",
                "#0F172A"),
            new Project(
                "Task Forge",
                "Project planning backend for teams",
                "https://github.com/example/task-forge",
                "#0EA5E9")
        ];

    await dbContext.Projects.AddRangeAsync(projects);
    await dbContext.SaveChangesAsync();

    Documentation[] documentations =
    [
      new Documentation(projects[0].Id, "Getting Started", "# Proyecto Atlas", 1, DocumentationKind.Page, DocumentationStatus.Draft, DocumentationArea.Backend),
      new Documentation(projects[0].Id, "ADR-001 Architecture", "## Layers", 2, DocumentationKind.Decision, DocumentationStatus.Published, DocumentationArea.Architecture),
      new Documentation(projects[1].Id, "Overview", "# Atlas Docs", 1, DocumentationKind.Note, DocumentationStatus.Archived, DocumentationArea.Frontend),
    ];

    await dbContext.Documentations.AddRangeAsync(documentations);
    await dbContext.SaveChangesAsync();
  }
}
