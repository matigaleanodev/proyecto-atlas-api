using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class DocumentationConfiguration : IEntityTypeConfiguration<Documentation>
{
  public void Configure(EntityTypeBuilder<Documentation> builder)
  {
    builder.ToTable("documentations");

    builder.HasKey(documentation => documentation.Id);

    builder.HasIndex(documentation => new { documentation.ProjectId, documentation.Slug })
        .IsUnique();

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(documentation => documentation.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(documentation => documentation.FaqItems)
        .WithOne()
        .HasForeignKey(faqItem => faqItem.DocumentationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(documentation => documentation.Tags)
        .WithOne()
        .HasForeignKey(tag => tag.DocumentationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(documentation => documentation.FaqItems)
        .UsePropertyAccessMode(PropertyAccessMode.Field);

    builder.Property(documentation => documentation.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(documentation => documentation.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(documentation => documentation.Title)
        .HasColumnName("title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(documentation => documentation.ContentMarkdown)
        .HasColumnName("content_markdown")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(documentation => documentation.Slug)
        .HasColumnName("slug")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(documentation => documentation.SortOrder)
        .HasColumnName("sort_order")
        .IsRequired();

    builder.Property(documentation => documentation.Kind)
        .HasColumnName("kind")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(documentation => documentation.Status)
        .HasColumnName("status")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(documentation => documentation.Area)
        .HasColumnName("area")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Navigation(documentation => documentation.Tags)
    .UsePropertyAccessMode(PropertyAccessMode.Field);

    builder.Property(documentation => documentation.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();

    builder.Property(documentation => documentation.UpdatedAtUtc)
        .HasColumnName("updated_at_utc")
        .IsRequired();
  }
}
