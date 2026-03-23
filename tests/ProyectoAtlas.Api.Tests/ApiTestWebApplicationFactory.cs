using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProyectoAtlas.Domain.Projects;
using ProyectoAtlas.Infrastructure.Persistence;

namespace ProyectoAtlas.Api.Tests;

public class ApiTestWebApplicationFactory : WebApplicationFactory<Program>
{
  private const string TestConnectionString =
      "Host=localhost;Port=5432;Database=atlas_test;Username=atlas;Password=atlas_dev_password";

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Development");

    builder.ConfigureServices(services =>
    {
      services.RemoveAll<DbContextOptions<ProyectoAtlasDbContext>>();

      services.AddDbContext<ProyectoAtlasDbContext>(options =>
              options.UseNpgsql(TestConnectionString));
    });
  }

  public async Task ResetDatabaseAsync()
  {
    using var scope = Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ProyectoAtlasDbContext>();

    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
  }

  public async Task SeedProjectsAsync()
  {
    using var scope = Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ProyectoAtlasDbContext>();

    var projects = new[]
    {
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
        };

    await dbContext.Projects.AddRangeAsync(projects);
    await dbContext.SaveChangesAsync();
  }
}
