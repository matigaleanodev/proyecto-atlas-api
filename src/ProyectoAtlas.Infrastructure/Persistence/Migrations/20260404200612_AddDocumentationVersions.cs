using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDocumentationVersions : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "documentation_versions",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          documentation_id = table.Column<Guid>(type: "uuid", nullable: false),
          version_number = table.Column<int>(type: "integer", nullable: false),
          title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
          content_markdown = table.Column<string>(type: "text", nullable: false),
          status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
          created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_documentation_versions", x => x.id);
          table.ForeignKey(
                    name: "FK_documentation_versions_documentations_documentation_id",
                    column: x => x.documentation_id,
                    principalTable: "documentations",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_documentation_versions_documentation_id_version_number",
        table: "documentation_versions",
        columns: new[] { "documentation_id", "version_number" },
        unique: true);
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "documentation_versions");
  }
}
