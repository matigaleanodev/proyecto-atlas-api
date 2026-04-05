using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class DocumentationResourceConfiguration : IEntityTypeConfiguration<DocumentationResource>
{
  public void Configure(EntityTypeBuilder<DocumentationResource> builder)
  {
    builder.ToTable("documentation_resources");

    builder.HasKey(resource => resource.Id);

    builder.HasIndex(resource => new
    {
      resource.DocumentationId,
      resource.NormalizedTitle,
      resource.NormalizedUrl,
      resource.Kind
    }).IsUnique();

    builder.HasOne<Documentation>()
        .WithMany()
        .HasForeignKey(resource => resource.DocumentationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(resource => resource.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(resource => resource.DocumentationId)
        .HasColumnName("documentation_id")
        .IsRequired();

    builder.Property(resource => resource.Title)
        .HasColumnName("title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(resource => resource.NormalizedTitle)
        .HasColumnName("normalized_title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(resource => resource.Url)
        .HasColumnName("url")
        .HasMaxLength(2048)
        .IsRequired();

    builder.Property(resource => resource.NormalizedUrl)
        .HasColumnName("normalized_url")
        .HasMaxLength(2048)
        .IsRequired();

    builder.Property(resource => resource.Kind)
        .HasColumnName("kind")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(resource => resource.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();
  }
}
