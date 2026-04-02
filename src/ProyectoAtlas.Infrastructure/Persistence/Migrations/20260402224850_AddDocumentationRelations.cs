using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDocumentationRelations : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "documentation_relations",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          project_id = table.Column<Guid>(type: "uuid", nullable: false),
          source_documentation_id = table.Column<Guid>(type: "uuid", nullable: false),
          target_documentation_id = table.Column<Guid>(type: "uuid", nullable: false),
          kind = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
          created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_documentation_relations", x => x.id);
          table.ForeignKey(
                    name: "FK_documentation_relations_documentations_source_documentation~",
                    column: x => x.source_documentation_id,
                    principalTable: "documentations",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
          table.ForeignKey(
                    name: "FK_documentation_relations_documentations_target_documentation~",
                    column: x => x.target_documentation_id,
                    principalTable: "documentations",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
          table.ForeignKey(
                    name: "FK_documentation_relations_projects_project_id",
                    column: x => x.project_id,
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_documentation_relations_project_id_source_documentation_id_~",
        table: "documentation_relations",
        columns: new[] { "project_id", "source_documentation_id", "target_documentation_id", "kind" },
        unique: true);

    migrationBuilder.CreateIndex(
        name: "IX_documentation_relations_source_documentation_id",
        table: "documentation_relations",
        column: "source_documentation_id");

    migrationBuilder.CreateIndex(
        name: "IX_documentation_relations_target_documentation_id",
        table: "documentation_relations",
        column: "target_documentation_id");
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "documentation_relations");
  }
}
