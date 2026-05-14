using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Features;
using ProyectoAtlas.Domain.Milestones;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class MilestoneFeatureLinkConfiguration : IEntityTypeConfiguration<MilestoneFeatureLink>
{
  public void Configure(EntityTypeBuilder<MilestoneFeatureLink> builder)
  {
    builder.ToTable("milestone_feature_links");

    builder.HasKey(link => link.Id);

    builder.HasIndex(link => new
    {
      link.ProjectId,
      link.MilestoneId,
      link.FeatureId
    }).IsUnique();

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(link => link.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Milestone>()
        .WithMany()
        .HasForeignKey(link => link.MilestoneId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Feature>()
        .WithMany()
        .HasForeignKey(link => link.FeatureId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(link => link.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(link => link.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(link => link.MilestoneId)
        .HasColumnName("milestone_id")
        .IsRequired();

    builder.Property(link => link.FeatureId)
        .HasColumnName("feature_id")
        .IsRequired();

    builder.Property(link => link.CreatedAtUtc)
        .HasColumnName("created_at_utc")
        .IsRequired();
  }
}
