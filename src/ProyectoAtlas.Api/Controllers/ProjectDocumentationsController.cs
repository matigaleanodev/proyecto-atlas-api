using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectDocumentationsController(
  CreateProjectDocumentationCommandHandler createProjectDocumentationCommandHandler,
  ListProjectDocumentationsQueryHandler listProjectDocumentationsQueryHandler,
  GetProjectDocumentationBySlugQueryHandler getProjectDocumentationBySlugQueryHandler,
  UpdateProjectDocumentationCommandHandler updateProjectDocumentationCommandHandler,
  DeleteProjectDocumentationCommandHandler deleteProjectDocumentationCommandHandler
  ) : ControllerBase
{

  [HttpPost]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateDocumentation(
      string projectSlug,
      [FromBody] CreateProjectDocumentationCommand command,
      CancellationToken cancellationToken = default)
  {
    Documentation documentation = await createProjectDocumentationCommandHandler.Execute(projectSlug, command, cancellationToken);

    return Created(
            $"/projects/{projectSlug}/documentations/{documentation.Slug}",
            documentation);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectDocumentationsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetDocumentations(
      string projectSlug,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      [FromQuery] DocumentationKind? kind = null,
      [FromQuery] DocumentationStatus? status = null,
      [FromQuery] DocumentationArea? area = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectDocumentationsQuery queryModel = new(page, pageSize, query, kind, status, area);
    ListProjectDocumentationsResponse response =
        await listProjectDocumentationsQueryHandler.Execute(projectSlug, queryModel, cancellationToken);

    return Ok(response);
  }

  [HttpGet("{slug}")]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetDocumentation(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    Documentation documentation = await getProjectDocumentationBySlugQueryHandler.Execute(projectSlug, slug, cancellationToken);
    return Ok(documentation);
  }

  [HttpPatch("{slug}")]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> UpdateDocumentation(
      string projectSlug,
      string slug,
      [FromBody] UpdateProjectDocumentationCommand command,
      CancellationToken cancellationToken = default)
  {
    Documentation documentation = await updateProjectDocumentationCommandHandler.Execute(projectSlug, slug, command, cancellationToken);
    return Ok(documentation);
  }

  [HttpDelete("{slug}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteDocumentation(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    await deleteProjectDocumentationCommandHandler.Execute(projectSlug, slug, cancellationToken);
    return NoContent();
  }
}
