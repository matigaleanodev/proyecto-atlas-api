using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Audit;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
  public void Configure(EntityTypeBuilder<AuditEvent> builder)
  {
    builder.ToTable("audit_events");

    builder.HasKey(auditEvent => auditEvent.Id);

    builder.HasIndex(auditEvent => new
    {
      auditEvent.ProjectId,
      auditEvent.OccurredAtUtc
    });

    builder.HasIndex(auditEvent => new
    {
      auditEvent.DocumentationId,
      auditEvent.OccurredAtUtc
    });

    builder.HasOne<Project>()
        .WithMany()
        .HasForeignKey(auditEvent => auditEvent.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Documentation>()
        .WithMany()
        .HasForeignKey(auditEvent => auditEvent.DocumentationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(auditEvent => auditEvent.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(auditEvent => auditEvent.ProjectId)
        .HasColumnName("project_id")
        .IsRequired();

    builder.Property(auditEvent => auditEvent.DocumentationId)
        .HasColumnName("documentation_id");

    builder.Property(auditEvent => auditEvent.EntityType)
        .HasColumnName("entity_type")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(auditEvent => auditEvent.EntityId)
        .HasColumnName("entity_id")
        .IsRequired();

    builder.Property(auditEvent => auditEvent.EntitySlug)
        .HasColumnName("entity_slug")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(auditEvent => auditEvent.EntityTitle)
        .HasColumnName("entity_title")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(auditEvent => auditEvent.Action)
        .HasColumnName("action")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(auditEvent => auditEvent.OccurredAtUtc)
        .HasColumnName("occurred_at_utc")
        .IsRequired();
  }
}
