using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Features;
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
    Type? enumType = Nullable.GetUnderlyingType(type) ?? type;

    if (enumType.IsEnum)
    {
      schema.Type = JsonSchemaType.String;
      schema.Format = null;
      schema.Pattern = null;
      schema.Enum =
      [
        .. Enum.GetNames(enumType)
            .Select(name => JsonValue.Create(name)!)
      ];
    }

    if (type == typeof(UpdateProjectCommand))
    {
      schema.Required?.Clear();
    }

    if (type == typeof(UpdateProjectFeatureCommand))
    {
      schema.Required?.Clear();
    }

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
      Type currentType when currentType == typeof(CreateProjectCommand) => ParseJson(
          """
          {
            "title": "Proyecto Atlas",
            "description": "Backend for project documentation based on markdown",
            "repositoryUrl": "https://github.com/matigaleanodev/proyecto-atlas-api",
            "color": "#1E293B",
            "links": [
              {
                "title": "Repository",
                "url": "https://github.com/matigaleanodev/proyecto-atlas-api",
                "description": "Main source code",
                "sortOrder": 1,
                "kind": "Repository"
              }
            ]
          }
          """),
      Type currentType when currentType == typeof(UpdateProjectCommand) => ParseJson(
          """
          {
            "title": "Atlas Platform",
            "description": "Updated backend for project documentation",
            "repositoryUrl": "https://github.com/matigaleanodev/atlas-platform",
            "color": "#0F172A",
            "links": [
              {
                "title": "Monitoring",
                "url": "https://grafana.example.com/atlas-platform",
                "description": "Operational dashboards",
                "sortOrder": 1,
                "kind": "Monitoring"
              }
            ]
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
            "links": [
              {
                "id": "4d987fe2-3489-46db-9b83-e493750a8d8e",
                "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
                "title": "Repository",
                "url": "https://github.com/matigaleanodev/proyecto-atlas-api",
                "description": "Main source code",
                "sortOrder": 1,
                "kind": "Repository"
              }
            ],
            "createdAtUtc": "2026-03-24T18:30:00Z",
            "updatedAtUtc": "2026-03-24T18:30:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListProjectsResponse) => ParseJson(
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
                "links": [
                  {
                    "id": "4d987fe2-3489-46db-9b83-e493750a8d8e",
                    "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
                    "title": "Repository",
                    "url": "https://github.com/matigaleanodev/proyecto-atlas-api",
                    "description": "Main source code",
                    "sortOrder": 1,
                    "kind": "Repository"
                  }
                ],
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
      Type currentType when currentType == typeof(CreateProjectFeatureCommand) => ParseJson(
          """
          {
            "title": "Authentication API",
            "summary": "Expose the first auth endpoints for Atlas clients.",
            "status": "Planned"
          }
          """),
      Type currentType when currentType == typeof(UpdateProjectFeatureCommand) => ParseJson(
          """
          {
            "title": "Authentication API",
            "summary": "Expose login and token refresh endpoints.",
            "status": "InProgress"
          }
          """),
      Type currentType when currentType == typeof(Feature) => ParseJson(
          """
          {
            "id": "b9a57f81-8729-4f58-a65a-4f0b7b8fc11d",
            "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
            "title": "Authentication API",
            "summary": "Expose the first auth endpoints for Atlas clients.",
            "slug": "authentication-api",
            "status": "Planned",
            "createdAtUtc": "2026-04-01T12:00:00Z",
            "updatedAtUtc": "2026-04-01T12:00:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListProjectFeaturesResponse) => ParseJson(
          """
          {
            "items": [
              {
                "id": "b9a57f81-8729-4f58-a65a-4f0b7b8fc11d",
                "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
                "title": "Authentication API",
                "summary": "Expose the first auth endpoints for Atlas clients.",
                "slug": "authentication-api",
                "status": "Planned",
                "createdAtUtc": "2026-04-01T12:00:00Z",
                "updatedAtUtc": "2026-04-01T12:00:00Z"
              }
            ],
            "page": 1,
            "pageSize": 10,
            "totalPages": 1,
            "totalItems": 1
          }
          """),
      Type currentType when currentType == typeof(CreateDocumentationRelationCommand) => ParseJson(
          """
          {
            "targetDocumentationSlug": "adr-001-architecture",
            "kind": "Implements"
          }
          """),
      Type currentType when currentType == typeof(DocumentationRelation) => ParseJson(
          """
          {
            "id": "57d10b57-d4cf-4771-bf4d-ea690fb07191",
            "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
            "sourceDocumentationId": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
            "targetDocumentationId": "96e358ab-8ea3-4030-b9f0-238ed459e537",
            "kind": "Implements",
            "createdAtUtc": "2026-04-02T12:00:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListDocumentationRelationsResponse) => ParseJson(
          """
          {
            "items": [
              {
                "id": "57d10b57-d4cf-4771-bf4d-ea690fb07191",
                "projectId": "8b658c72-8f6f-4fef-9d65-f2fa6eb60bd7",
                "sourceDocumentationId": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
                "targetDocumentationId": "96e358ab-8ea3-4030-b9f0-238ed459e537",
                "kind": "Implements",
                "createdAtUtc": "2026-04-02T12:00:00Z"
              }
            ]
          }
          """),
      Type currentType when currentType == typeof(CreateDocumentationResourceCommand) => ParseJson(
          """
          {
            "title": "OpenAPI Spec",
            "url": "https://api.example.com/openapi.json",
            "kind": "ApiSpec"
          }
          """),
      Type currentType when currentType == typeof(DocumentationResource) => ParseJson(
          """
          {
            "id": "e8d6b8d1-6610-4ebf-a4f3-a264cb14b512",
            "documentationId": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
            "title": "OpenAPI Spec",
            "url": "https://api.example.com/openapi.json",
            "kind": "ApiSpec",
            "createdAtUtc": "2026-04-05T12:00:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListDocumentationResourcesResponse) => ParseJson(
          """
          {
            "items": [
              {
                "id": "e8d6b8d1-6610-4ebf-a4f3-a264cb14b512",
                "documentationId": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
                "title": "OpenAPI Spec",
                "url": "https://api.example.com/openapi.json",
                "kind": "ApiSpec",
                "createdAtUtc": "2026-04-05T12:00:00Z"
              }
            ]
          }
          """),
      Type currentType when currentType == typeof(DocumentationVersion) => ParseJson(
          """
          {
            "id": "3f373334-b57e-489d-8f1a-0d6eb0aee149",
            "documentationId": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
            "versionNumber": 2,
            "title": "Getting Started",
            "contentMarkdown": "# Proyecto Atlas\n\nGuia inicial anterior.",
            "status": "Draft",
            "createdAtUtc": "2026-04-04T12:00:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListDocumentationVersionsResponse) => ParseJson(
          """
          {
            "items": [
              {
                "id": "3f373334-b57e-489d-8f1a-0d6eb0aee149",
                "documentationId": "d7f2cb87-86bc-4c25-bb77-27e4f7c38b67",
                "versionNumber": 2,
                "title": "Getting Started",
                "contentMarkdown": "# Proyecto Atlas\n\nGuia inicial anterior.",
                "status": "Draft",
                "createdAtUtc": "2026-04-04T12:00:00Z"
              }
            ]
          }
          """),
      Type currentType when currentType == typeof(CreateProjectDocumentationCommand) => ParseJson(
          """
          {
            "title": "Getting Started",
            "contentMarkdown": "# Proyecto Atlas\n\nGuia inicial para levantar el backend localmente.",
            "sortOrder": 1,
            "kind": "Page",
            "status": "Draft"
          }
          """),
      Type currentType when currentType == typeof(UpdateProjectDocumentationCommand) => ParseJson(
          """
          {
            "title": "Quick Start",
            "contentMarkdown": "## Updated\n\nPasos minimos para correr el proyecto.",
            "sortOrder": 2,
            "status": "Published"
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
            "kind": "Page",
            "status": "Draft",
            "createdAtUtc": "2026-03-24T18:35:00Z",
            "updatedAtUtc": "2026-03-24T18:35:00Z"
          }
          """),
      Type currentType when currentType == typeof(ListProjectDocumentationsResponse) => ParseJson(
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
                "kind": "Page",
                "status": "Draft",
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
