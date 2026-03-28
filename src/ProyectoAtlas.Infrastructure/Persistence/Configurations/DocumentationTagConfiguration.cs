using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class DocumentationTagConfiguration : IEntityTypeConfiguration<DocumentationTag>
{
  public void Configure(EntityTypeBuilder<DocumentationTag> builder)
  {
    builder.ToTable("documentation_tags");

    builder.HasKey(documentationTag => documentationTag.Id);

    builder.HasIndex(documentationTag => new { documentationTag.DocumentationId, documentationTag.NormalizedName })
        .IsUnique();

    builder.Property(documentationTag => documentationTag.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(documentationTag => documentationTag.DocumentationId)
        .HasColumnName("documentation_id")
        .IsRequired();

    builder.Property(documentationTag => documentationTag.Name)
        .HasColumnName("name")
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(documentationTag => documentationTag.NormalizedName)
        .HasColumnName("normalized_name")
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(documentationTag => documentationTag.CanonicalName)
        .HasColumnName("canonical_name")
        .HasMaxLength(100)
        .IsRequired();

  }
}
