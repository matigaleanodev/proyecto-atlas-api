using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDocumentations : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "documentations",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          project_id = table.Column<Guid>(type: "uuid", nullable: false),
          title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
          slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
          content_markdown = table.Column<string>(type: "text", nullable: false),
          sort_order = table.Column<int>(type: "integer", nullable: false),
          created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
          updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_documentations", x => x.id);
          table.ForeignKey(
                    name: "FK_documentations_projects_project_id",
                    column: x => x.project_id,
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_documentations_project_id_slug",
        table: "documentations",
        columns: ["project_id", "slug"],
        unique: true);
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "documentations");
  }
}
