using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProjectRelations : Migration
{
  private static readonly string[] ProjectRelationIndexColumns = ["source_project_id", "target_project_id", "kind"];

  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "project_relations",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          source_project_id = table.Column<Guid>(type: "uuid", nullable: false),
          target_project_id = table.Column<Guid>(type: "uuid", nullable: false),
          kind = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
          created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_project_relations", x => x.id);
          table.ForeignKey(
                    name: "FK_project_relations_projects_source_project_id",
                    column: x => x.source_project_id,
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
          table.ForeignKey(
                    name: "FK_project_relations_projects_target_project_id",
                    column: x => x.target_project_id,
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
        });

    migrationBuilder.CreateIndex(
        name: "IX_project_relations_source_project_id_target_project_id_kind",
        table: "project_relations",
        columns: ProjectRelationIndexColumns,
        unique: true);

    migrationBuilder.CreateIndex(
        name: "IX_project_relations_target_project_id",
        table: "project_relations",
        column: "target_project_id");
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "project_relations");
  }
}
