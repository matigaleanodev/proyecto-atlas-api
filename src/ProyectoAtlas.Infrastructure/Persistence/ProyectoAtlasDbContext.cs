using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;
namespace ProyectoAtlas.Infrastructure.Persistence;

public class ProyectoAtlasDbContext(DbContextOptions<ProyectoAtlasDbContext> options) : DbContext(options)
{
  public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<ProjectRelation> ProjectRelations => Set<ProjectRelation>();
  public DbSet<ProjectLink> ProjectLinks => Set<ProjectLink>();
  public DbSet<Documentation> Documentations => Set<Documentation>();
  public DbSet<DocumentationResource> DocumentationResources => Set<DocumentationResource>();
  public DbSet<DocumentationRelation> DocumentationRelations => Set<DocumentationRelation>();
  public DbSet<DocumentationVersion> DocumentationVersions => Set<DocumentationVersion>();
  public DbSet<DocumentationFaqItem> DocumentationFaqItems => Set<DocumentationFaqItem>();
  public DbSet<DocumentationTag> DocumentationTags => Set<DocumentationTag>();
  public DbSet<Feature> Features => Set<Feature>();
  public DbSet<FeatureDocumentationLink> FeatureDocumentationLinks => Set<FeatureDocumentationLink>();
  public DbSet<Milestone> Milestones => Set<Milestone>();
  public DbSet<MilestoneFeatureLink> MilestoneFeatureLinks => Set<MilestoneFeatureLink>();


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProyectoAtlasDbContext).Assembly);
  }
}
