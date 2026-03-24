# Roadmap

Estado general: active

Este archivo es local y operativo. Se usa solo para seguir el trabajo vigente del repo.

Regla de trabajo:

- Marcar cada tarea completada con `[x]`
- Marcar tareas activas con `[~]`
- Dejar pendientes con `[ ]`
- Mantener este archivo corto y mover decisiones duraderas a documentacion real cuando dejen de ser trabajo vigente
- No convertir este archivo en backlog eterno ni en documentacion de producto

## Fase 0 - Fundacion tecnica

Estado: active
Impact: high
Repositorio: proyecto-atlas-api

Objetivo:
Cerrar una base tecnica seria para Atlas antes de entrar en desarrollo de features o persistencia real.

Tareas:

- [x] Confirmar entorno base: .NET 10 SDK, Docker Desktop, PostgreSQL en contenedor y cliente visual para base local
- [x] Fijar version del stack con `global.json` y resolver el repo sobre .NET 10
- [x] Definir stack exacto de Fase 0: ASP.NET Core Web API, PostgreSQL y EF Core como camino estandar previsto
- [x] Definir estrategia inicial de testing: xUnit, unit tests por capa y integration tests para la API
- [x] Crear solucion base y primer proyecto `ProyectoAtlas.Api`
- [x] Limpiar el template inicial y dejar un primer endpoint operativo de health
- [x] Crear `ProyectoAtlas.Domain` y `ProyectoAtlas.Application` como base arquitectonica minima
- [x] Conectar `Api -> Application` y `Application -> Domain`
- [x] Agregar primeros tests reales: integration tests de API y unit test de `HealthCheckUseCase`
- [x] Delimitar el dominio inicial: Atlas arranca como backend para generar documentacion de proyectos en base a markdown
- [x] Recortar el bounded context inicial alrededor de `Project` como entidad central
- [x] Definir atributos nucleo iniciales de `Project`: title, description, repository, tags, color y links tipados
- [x] Definir primer corte del modelo relacional: `projects` y `project_links`
- [x] Definir estrategia exacta de persistencia con EF Core: `DbContext`, mappings y migrations iniciales
- [x] Diseñar la estructura interna minima de carpetas por capa antes del primer slice real de dominio
- [x] Definir el primer backend slice real sobre `Project`, pequeno, vertical y testeable
- [x] Cerrar CRUD inicial de `Project`: create, list paginado, get by slug, patch y delete con cobertura real
- [x] Definir como entra y se representa `Documentation` sin inflar `Project`
- [x] Cerrar CRUD inicial de `Documentation` anidado a `Project`: create, list paginado, get by slug, patch y delete con cobertura real
- [x] Decidir Swagger solo para development por ahora; exposicion publica queda pendiente de decision futura
- [x] Agregar Swagger UI para development y generacion automatica de `openapi.json` en build
- [x] Definir baseline inicial de lint/analyzers y formateo para .NET alineado con el resto del workspace
- [x] Definir manejo consistente de conflictos de unicidad y errores tecnicos de persistencia en endpoints de `Project` y `Documentation`
- [ ] Corregir backfill de `created_at_utc`, `updated_at_utc` y `slug` para registros legacy creados antes de esos campos
- [ ] Optimizar setup de integration tests de API para reducir costo y verbosidad del reset completo de base `atlas_test`

Done when:

- El repo tiene entorno estable, solucion limpia, testing inicial real, CRUD base de `Project` y `Documentation`, y un manejo suficientemente claro de errores/contratos para abrir el siguiente slice sin sobredisenar
