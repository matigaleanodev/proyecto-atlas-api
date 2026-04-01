using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
  public void Configure(EntityTypeBuilder<Feature> builder)
  {
    builder.ToTable("features");

    builder.HasKey(feature => feature.Id);

    builder.HasIndex(feature => new { feature.ProjectId, feature.Slug })
        .IsUnique();

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(feature => feature.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(feature => feature.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(feature => feature.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(feature => feature.Title)
        .HasColumnName("title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(feature => feature.Summary)
        .HasColumnName("summary")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(feature => feature.Slug)
        .HasColumnName("slug")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(feature => feature.Status)
        .HasColumnName("status")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(feature => feature.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();

    builder.Property(feature => feature.UpdatedAtUtc)
        .HasColumnName("updated_at_utc")
        .IsRequired();
  }
}
