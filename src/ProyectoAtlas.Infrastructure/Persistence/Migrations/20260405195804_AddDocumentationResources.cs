using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDocumentationResources : Migration
{
  private static readonly string[] DocumentationResourceIndexColumns =
      ["documentation_id", "normalized_title", "normalized_url", "kind"];

  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "documentation_resources",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          documentation_id = table.Column<Guid>(type: "uuid", nullable: false),
          title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
          normalized_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
          url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
          normalized_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
          kind = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
          created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_documentation_resources", x => x.id);
          table.ForeignKey(
                    name: "FK_documentation_resources_documentations_documentation_id",
                    column: x => x.documentation_id,
                    principalTable: "documentations",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_documentation_resources_documentation_id_normalized_title_n~",
        table: "documentation_resources",
        columns: DocumentationResourceIndexColumns,
        unique: true);
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "documentation_resources");
  }
}
