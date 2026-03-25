# Proyecto Atlas API

Backend inicial de Atlas para gestionar proyectos de software y su documentación en Markdown.

El repositorio está en Fase 0. El foco actual es cerrar una base técnica simple, mantenible y con contratos explícitos antes de abrir nuevos slices.

## Stack

- .NET 10
- ASP.NET Core Web API
- PostgreSQL
- EF Core
- xUnit

## Alcance actual

La API expone dos recursos:

- `Project`
- `Documentation`, anidado dentro de `Project`

Reglas vigentes:

- `Project.slug` es único globalmente
- `Documentation.slug` es único dentro de cada proyecto
- `Documentation.kind` clasifica el contenido documental y hoy admite `Page`, `Decision`, `Note`, `FAQ` y `ReleaseNotes`
- `Documentation.kind` se define al crear y no se edita después
- `Documentation.status` hoy admite `Draft`, `Published` y `Archived`
- si `Documentation.kind == Decision`, el título debe seguir la convención `ADR-001 Title`

## Endpoints

### Health

- `GET /health`

### Projects

- `POST /projects`
- `GET /projects`
- `GET /projects/{slug}`
- `PATCH /projects/{slug}`
- `DELETE /projects/{slug}`

### Project documentations

- `POST /projects/{projectSlug}/documentations`
- `GET /projects/{projectSlug}/documentations`
- `GET /projects/{projectSlug}/documentations/{slug}`
- `PATCH /projects/{projectSlug}/documentations/{slug}`
- `DELETE /projects/{projectSlug}/documentations/{slug}`

El listado soporta filtros opcionales por `query`, `kind` y `status`.

## OpenAPI y Swagger

En `Development`:

- OpenAPI JSON: `/openapi/v1.json`
- Swagger UI: `/swagger/index.html`

El archivo `openapi.json` también se genera en la raíz del repo durante el build de la API.
Swagger ya expone ejemplos legibles en los modelos principales de entrada, salida y error.

## Contrato de errores

Todos los errores del API devuelven el mismo shape:

```json
{
  "statusCode": 409,
  "message": "Project slug 'proyecto-atlas' already exists.",
  "code": "PROJECT_SLUG_CONFLICT"
}
```

Semántica:

- `statusCode`: código HTTP real de la respuesta
- `message`: mensaje orientado al desarrollo y debugging
- `code`: código estable para que frontend pueda decidir feedback y flujos

### Códigos actuales

- `PROJECT_NOT_FOUND`
- `PROJECT_SLUG_CONFLICT`
- `DOCUMENTATION_NOT_FOUND`
- `DOCUMENTATION_SLUG_CONFLICT`
- `DOCUMENTATION_TITLE_CONVENTION_INVALID`
- `VALIDATION_ERROR`
- `INTERNAL_SERVER_ERROR`

### Criterio actual

- errores conocidos de dominio o aplicación: devuelven `statusCode + message + code`
- errores de validación automática del framework: se normalizan al mismo contrato
- errores no mapeados: devuelven `500` con `INTERNAL_SERVER_ERROR`

## Payloads mínimos

### Crear proyecto

`POST /projects`

```json
{
  "title": "Proyecto Atlas",
  "description": "Backend for project documentation based on markdown",
  "repositoryUrl": "https://github.com/matigaleanodev/proyecto-atlas-api",
  "color": "#1E293B"
}
```

### Actualizar proyecto

`PATCH /projects/{slug}`

```json
{
  "title": "Atlas Platform",
  "description": "Updated backend for project documentation",
  "repositoryUrl": "https://github.com/example/atlas-platform",
  "color": "#0F172A"
}
```

Todos los campos son opcionales.

### Crear documentación

`POST /projects/{projectSlug}/documentations`

```json
{
  "title": "Getting Started",
  "contentMarkdown": "# Proyecto Atlas",
  "sortOrder": 1,
  "kind": "Page",
  "status": "Draft"
}
```

### Actualizar documentación

`PATCH /projects/{projectSlug}/documentations/{slug}`

```json
{
  "title": "Quick Start",
  "contentMarkdown": "## Updated",
  "sortOrder": 2,
  "status": "Published"
}
```

Todos los campos son opcionales.

## Desarrollo local

### Requisitos

- .NET 10 SDK
- PostgreSQL disponible localmente

### Variables esperadas

La API usa la connection string `DefaultConnection`.

En desarrollo local suele resolverse desde `appsettings.Development.json` o desde configuración del entorno, pero ese archivo no se versiona.

## Validación del repo

Antes de push o PR:

```powershell
dotnet format ProyectoAtlas.slnx --verify-no-changes
dotnet test ProyectoAtlas.slnx
```

Si necesitás corregir formato:

```powershell
dotnet format ProyectoAtlas.slnx
```

## Estado actual

Hasta este punto, Atlas ya tiene:

- health check operativo
- CRUD base de proyectos
- CRUD base de documentación anidada
- clasificacion inicial de `Documentation` mediante `kind`
- OpenAPI y Swagger en desarrollo
- CI base
- contrato uniforme de errores para toda la API
