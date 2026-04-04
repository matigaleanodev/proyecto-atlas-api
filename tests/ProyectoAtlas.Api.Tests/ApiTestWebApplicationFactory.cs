using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
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
        TRUNCATE TABLE documentation_versions, documentation_relations, features, documentations, projects RESTART IDENTITY CASCADE;
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
                "#1E293B",
                [
                  new ProjectLinkData("Repository", "https://github.com/matigaleanodev/proyecto-atlas-api", "Main source code", 1, ProjectLinkKind.Repository),
                  new ProjectLinkData("Board", "https://linear.app/proyecto-atlas", "Work tracking board", 2, ProjectLinkKind.Board)
                ]),
            new Project(
                "Atlas Docs",
                "Documentation explorer and reader",
                "https://github.com/matigaleanodev/atlas-docs",
                "#0F172A",
                [
                  new ProjectLinkData("Documentation", "https://docs.example.com/atlas-docs", "Published docs site", 1, ProjectLinkKind.Documentation)
                ]),
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
      new Documentation(
          projects[0].Id,
          "Getting Started",
          "# Proyecto Atlas",
          1,
          DocumentationKind.Page,
          DocumentationStatus.Draft,
          DocumentationArea.Backend,
          [new DocumentationTagData("backend"), new DocumentationTagData("dotnet")]),
      new Documentation(
          projects[0].Id,
          "ADR-001 Architecture",
          "## Layers",
          2,
          DocumentationKind.Decision,
          DocumentationStatus.Published,
          DocumentationArea.Architecture,
          [new DocumentationTagData("architecture"), new DocumentationTagData("adr")]),
      new Documentation(
          projects[1].Id,
          "Overview",
          "# Atlas Docs",
          1,
          DocumentationKind.Note,
          DocumentationStatus.Archived,
          DocumentationArea.Frontend,
          [new DocumentationTagData("frontend")]),
    ];

    await dbContext.Documentations.AddRangeAsync(documentations);
    await dbContext.SaveChangesAsync();

    DocumentationRelation[] documentationRelations =
    [
      new DocumentationRelation(
          projects[0].Id,
          documentations[0].Id,
          documentations[1].Id,
          DocumentationRelationKind.Implements)
    ];

    await dbContext.DocumentationRelations.AddRangeAsync(documentationRelations);
    await dbContext.SaveChangesAsync();

    Feature[] features =
    [
      new Feature(
          projects[0].Id,
          "Authentication API",
          "Expose login endpoints.",
          FeatureStatus.Planned),
      new Feature(
          projects[0].Id,
          "Documentation Search",
          "Index technical content for search.",
          FeatureStatus.InProgress),
      new Feature(
          projects[1].Id,
          "Reader UI",
          "Render published content for readers.",
          FeatureStatus.Done)
    ];

    await dbContext.Features.AddRangeAsync(features);
    await dbContext.SaveChangesAsync();
  }
}
