using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAtlas.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDocumentationResourceMetadata : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.AddColumn<string>(
        name: "description",
        table: "documentation_resources",
        type: "character varying(280)",
        maxLength: 280,
        nullable: true);

    migrationBuilder.AddColumn<int>(
        name: "sort_order",
        table: "documentation_resources",
        type: "integer",
        nullable: false,
        defaultValue: 1);
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropColumn(
        name: "description",
        table: "documentation_resources");

    migrationBuilder.DropColumn(
        name: "sort_order",
        table: "documentation_resources");
  }
}
