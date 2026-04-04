using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class DocumentationVersionConfiguration : IEntityTypeConfiguration<DocumentationVersion>
{
  public void Configure(EntityTypeBuilder<DocumentationVersion> builder)
  {
    builder.ToTable("documentation_versions");

    builder.HasKey(version => version.Id);

    builder.HasIndex(version => new { version.DocumentationId, version.VersionNumber })
        .IsUnique();

    builder.HasOne<Documentation>()
        .WithMany()
        .HasForeignKey(version => version.DocumentationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(version => version.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(version => version.DocumentationId)
        .HasColumnName("documentation_id")
        .IsRequired();

    builder.Property(version => version.VersionNumber)
        .HasColumnName("version_number")
        .IsRequired();

    builder.Property(version => version.Title)
        .HasColumnName("title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(version => version.ContentMarkdown)
        .HasColumnName("content_markdown")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(version => version.Status)
        .HasColumnName("status")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(version => version.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();
  }
}
