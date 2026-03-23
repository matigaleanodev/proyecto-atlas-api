using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(project => project.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(project => project.RepositoryUrl)
            .HasColumnName("repository_url")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(project => project.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.Color)
            .HasColumnName("color")
            .HasMaxLength(9)
            .IsRequired();

        builder.Property(project => project.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(project => project.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();
    }
}
