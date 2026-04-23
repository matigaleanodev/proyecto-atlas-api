using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureDocumentationLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "feature_documentation_links",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    documentation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feature_documentation_links", x => x.id);
                    table.ForeignKey(
                        name: "FK_feature_documentation_links_documentations_documentation_id",
                        column: x => x.documentation_id,
                        principalTable: "documentations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feature_documentation_links_features_feature_id",
                        column: x => x.feature_id,
                        principalTable: "features",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feature_documentation_links_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_feature_documentation_links_documentation_id",
                table: "feature_documentation_links",
                column: "documentation_id");

            migrationBuilder.CreateIndex(
                name: "IX_feature_documentation_links_feature_id",
                table: "feature_documentation_links",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "IX_feature_documentation_links_project_id_feature_id_documenta~",
                table: "feature_documentation_links",
                columns: new[] { "project_id", "feature_id", "documentation_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feature_documentation_links");
        }
    }
}
