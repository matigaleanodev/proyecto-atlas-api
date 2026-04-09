using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
  public void Configure(EntityTypeBuilder<Milestone> builder)
  {
    builder.ToTable("milestones");

    builder.HasKey(milestone => milestone.Id);

    builder.HasIndex(milestone => new { milestone.ProjectId, milestone.Slug })
        .IsUnique();

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(milestone => milestone.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(milestone => milestone.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(milestone => milestone.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(milestone => milestone.Title)
        .HasColumnName("title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(milestone => milestone.Summary)
        .HasColumnName("summary")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(milestone => milestone.Slug)
        .HasColumnName("slug")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(milestone => milestone.Status)
        .HasColumnName("status")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(milestone => milestone.TargetDateUtc)
        .HasColumnName("target_date_utc");

    builder.Property(milestone => milestone.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();

    builder.Property(milestone => milestone.UpdatedAtUtc)
        .HasColumnName("updated_at_utc")
        .IsRequired();
  }
}
