using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Infrastructure.Persistence.Configurations;

public class DocumentationFaqItemConfiguration : IEntityTypeConfiguration<DocumentationFaqItem>
{
  public void Configure(EntityTypeBuilder<DocumentationFaqItem> builder)
  {
    builder.ToTable("documentation_faq_items");

    builder.HasKey(faqItem => faqItem.Id);

    builder.HasIndex(faqItem => new { faqItem.DocumentationId, faqItem.SortOrder })
        .IsUnique();

    builder.Property(faqItem => faqItem.Id)
        .HasColumnName("id")
        .ValueGeneratedNever();

    builder.Property(faqItem => faqItem.DocumentationId)
        .HasColumnName("documentation_id")
        .IsRequired();

    builder.Property(faqItem => faqItem.Question)
        .HasColumnName("question")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(faqItem => faqItem.Answer)
        .HasColumnName("answer")
        .HasColumnType("text")
        .IsRequired();

    builder.Property(faqItem => faqItem.SortOrder)
        .HasColumnName("sort_order")
        .IsRequired();
  }
}
