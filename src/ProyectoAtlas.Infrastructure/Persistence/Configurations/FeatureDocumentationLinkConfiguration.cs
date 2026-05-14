using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class FeatureDocumentationLinkConfiguration : IEntityTypeConfiguration<FeatureDocumentationLink>
{
  public void Configure(EntityTypeBuilder<FeatureDocumentationLink> builder)
  {
    builder.ToTable("feature_documentation_links");

    builder.HasKey(link => link.Id);

    builder.HasIndex(link => new
    {
      link.ProjectId,
      link.FeatureId,
      link.DocumentationId
    }).IsUnique();

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(link => link.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Feature>()
        .WithMany()
        .HasForeignKey(link => link.FeatureId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Documentation>()
        .WithMany()
        .HasForeignKey(link => link.DocumentationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(link => link.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(link => link.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(link => link.FeatureId)
        .HasColumnName("feature_id")
        .IsRequired();

    builder.Property(link => link.DocumentationId)
        .HasColumnName("documentation_id")
        .IsRequired();

    builder.Property(link => link.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();
  }
}
