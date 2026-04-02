using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class DocumentationRelationConfiguration : IEntityTypeConfiguration<DocumentationRelation>
{
  public void Configure(EntityTypeBuilder<DocumentationRelation> builder)
  {
    builder.ToTable("documentation_relations");

    builder.HasKey(relation => relation.Id);

    builder.HasIndex(relation => new
    {
      relation.ProjectId,
      relation.SourceDocumentationId,
      relation.TargetDocumentationId,
      relation.Kind
    }).IsUnique();

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(relation => relation.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Documentation>()
        .WithMany()
        .HasForeignKey(relation => relation.SourceDocumentationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Documentation>()
        .WithMany()
        .HasForeignKey(relation => relation.TargetDocumentationId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Property(relation => relation.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(relation => relation.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(relation => relation.SourceDocumentationId)
        .HasColumnName("source_documentation_id")
        .IsRequired();

    builder.Property(relation => relation.TargetDocumentationId)
        .HasColumnName("target_documentation_id")
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
