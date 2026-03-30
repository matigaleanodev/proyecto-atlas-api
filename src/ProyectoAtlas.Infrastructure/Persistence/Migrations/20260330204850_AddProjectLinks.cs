using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProjectLinks : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "project_links",
        columns: table => new
        {
          id = table.Column<Guid>(type: "uuid", nullable: false),
          project_id = table.Column<Guid>(type: "uuid", nullable: false),
          title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
          sort_order = table.Column<int>(type: "integer", nullable: false),
          description = table.Column<string>(type: "text", nullable: false),
          url = table.Column<string>(type: "text", nullable: false),
          kind = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_project_links", x => x.id);
          table.ForeignKey(
                    name: "FK_project_links_projects_project_id",
                    column: x => x.project_id,
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_project_links_project_id_sort_order",
        table: "project_links",
        columns: new[] { "project_id", "sort_order" },
        unique: true);
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "project_links");
  }
}
