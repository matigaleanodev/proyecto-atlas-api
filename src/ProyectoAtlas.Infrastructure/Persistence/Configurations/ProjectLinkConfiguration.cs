using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class ProjectLinkConfiguration : IEntityTypeConfiguration<ProjectLink>
{
  public void Configure(EntityTypeBuilder<ProjectLink> builder)
  {
    builder.ToTable("project_links");

    builder.HasKey(link => link.Id);

    builder.HasIndex(link => new { link.ProjectId, link.SortOrder })
        .IsUnique();

    builder.Property(link => link.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(link => link.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(link => link.Title)
        .HasColumnName("title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(link => link.Description)
        .HasColumnName("description")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(link => link.Url)
        .HasColumnName("url")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(link => link.SortOrder)
        .HasColumnName("sort_order")
        .IsRequired();

    builder.Property(link => link.Kind)
        .HasColumnName("kind")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();
  }
}
