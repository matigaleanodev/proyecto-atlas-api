using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;
namespace ProyectoAtlas.Infrastructure.Persistence;

public class ProyectoAtlasDbContext(DbContextOptions<ProyectoAtlasDbContext> options) : DbContext(options)
{
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<Documentation> Documentations => Set<Documentation>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProyectoAtlasDbContext).Assembly);
  }
}
