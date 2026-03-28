using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDocumentationFaqItems : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "documentation_faq_items",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          documentation_id = table.Column<Guid>(type: "uuid", nullable: false),
          question = table.Column<string>(type: "text", nullable: false),
          answer = table.Column<string>(type: "text", nullable: false),
          sort_order = table.Column<int>(type: "integer", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_documentation_faq_items", x => x.id);
          table.ForeignKey(
              name: "FK_documentation_faq_items_documentations_documentation_id",
              column: x => x.documentation_id,
              principalTable: "documentations",
              principalColumn: "id",
              onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_documentation_faq_items_documentation_id_sort_order",
        table: "documentation_faq_items",
        columns: ["documentation_id", "sort_order"],
        unique: true);
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "documentation_faq_items");
  }
}
