using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.OpenApi;

public static class OpenApiExampleTransformers
{
  public static Task ApplyExamples(
      Microsoft.OpenApi.OpenApiSchema schema,
      OpenApiSchemaTransformerContext context,
      CancellationToken cancellationToken)
  {
    Type? type = context.JsonTypeInfo.Type;

    schema.Example = type switch
    {
      Type currentType when currentType == typeof(ApiErrorResponse) => ParseJson(
          """
          {
            "statusCode": 409,
            "message": "Project slug 'proyecto-atlas' already exists.",
            "code": "PROJECT_SLUG_CONFLICT"
          }
          """),
      Type currentType when currentType == typeof(CreateProjectInput) => ParseJson(
          """
          {
            "title": "Proyecto Atlas",
            "description": "Backend for project documentation based on markdown",
            "repositoryUrl": "https://github.com/matigaleanodev/proyecto-atlas-api",
            "color": "#1E293B"
          }
          """),
      Type currentType when currentType == typeof(UpdateProjectInput) => ParseJson(
          """
          {
            "title": "Atlas Platform",
            "description": "Updated backend for project documentation",
            "repositoryUrl": "https://github.com/matigaleanodev/atlas-platform",
            "color": "#0F172A"
          }
          """),
      Type currentType when currentType == typeof(Project) => ParseJson(
          """
          {
            "id": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
            "title": "Proyecto Atlas",
            "description": "Backend for project documentation based on markdown",
            "repositoryUrl": "https://github.com/matigaleanodev/proyecto-atlas-api",
            "color": "#1E293B",
            "slug": "proyecto-atlas",
            "createdAtUtc": "2026-03-24T18:30:00Z",
            "updatedAtUtc": "2026-03-24T18:30:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListProjectsOutput) => ParseJson(
          """
          {
            "items": [
              {
                "id": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
                "title": "Proyecto Atlas",
                "description": "Backend for project documentation based on markdown",
                "repositoryUrl": "https://github.com/matigaleanodev/proyecto-atlas-api",
                "color": "#1E293B",
                "slug": "proyecto-atlas",
                "createdAtUtc": "2026-03-24T18:30:00Z",
                "updatedAtUtc": "2026-03-24T18:30:00Z"
              }
            ],
            "page": 1,
            "pageSize": 10,
            "totalPages": 1,
            "totalItems": 1
          }
          """),
      Type currentType when currentType == typeof(CreateProjectDocumentationInput) => ParseJson(
          """
          {
            "title": "Getting Started",
            "contentMarkdown": "# Proyecto Atlas\n\nGuia inicial para levantar el backend localmente.",
            "sortOrder": 1
          }
          """),
      Type currentType when currentType == typeof(UpdateProjectDocumentationInput) => ParseJson(
          """
          {
            "title": "Quick Start",
            "contentMarkdown": "## Updated\n\nPasos minimos para correr el proyecto.",
            "sortOrder": 2
          }
          """),
      Type currentType when currentType == typeof(Documentation) => ParseJson(
          """
          {
            "id": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
            "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
            "title": "Getting Started",
            "slug": "getting-started",
            "contentMarkdown": "# Proyecto Atlas\n\nGuia inicial para levantar el backend localmente.",
            "sortOrder": 1,
            "createdAtUtc": "2026-03-24T18:35:00Z",
            "updatedAtUtc": "2026-03-24T18:35:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListProjectDocumentationsOutput) => ParseJson(
          """
          {
            "items": [
              {
                "id": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
                "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
                "title": "Getting Started",
                "slug": "getting-started",
                "contentMarkdown": "# Proyecto Atlas\n\nGuia inicial para levantar el backend localmente.",
                "sortOrder": 1,
                "createdAtUtc": "2026-03-24T18:35:00Z",
                "updatedAtUtc": "2026-03-24T18:35:00Z"
              }
            ],
            "page": 1,
            "pageSize": 10,
            "totalPages": 1,
            "totalItems": 1
          }
          """),
      _ => schema.Example
    };

    return Task.CompletedTask;
  }

  private static JsonNode ParseJson(string json)
  {
    return JsonNode.Parse(json)!;
  }
}
