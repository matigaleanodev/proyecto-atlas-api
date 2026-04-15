using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class ProjectRelationConfiguration : IEntityTypeConfiguration<ProjectRelation>
{
  public void Configure(EntityTypeBuilder<ProjectRelation> builder)
  {
    builder.ToTable("project_relations");

    builder.HasKey(relation => relation.Id);

    builder.HasIndex(relation => new
    {
      relation.SourceProjectId,
      relation.TargetProjectId,
      relation.Kind
    }).IsUnique();

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(relation => relation.SourceProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(relation => relation.TargetProjectId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Property(relation => relation.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(relation => relation.SourceProjectId)
        .HasColumnName("source_project_id")
        .IsRequired();

    builder.Property(relation => relation.TargetProjectId)
        .HasColumnName("target_project_id")
        .IsRequired();

    builder.Property(relation => relation.Kind)
        .HasColumnName("kind")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(relation => relation.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();
  }
}
