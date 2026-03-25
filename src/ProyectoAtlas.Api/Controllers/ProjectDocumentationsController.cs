using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectDocumentationsController(
  CreateProjectDocumentationUseCase createDocumentationUseCase,
  ListProjectDocumentationsUseCase listProjectDocumentationsUseCase,
  GetProjectDocumentationBySlugUseCase getProjectDocumentationBySlugUseCase,
  UpdateProjectDocumentationUseCase updateProjectDocumentationUseCase,
  DeleteProjectDocumentationUseCase deleteProjectDocumentationUseCase
  ) : ControllerBase
{

  [HttpPost]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateDocumentation(
      string projectSlug,
      [FromBody] CreateProjectDocumentationInput input,
      CancellationToken cancellationToken = default)
  {
    Documentation documentation = await createDocumentationUseCase.Execute(projectSlug, input, cancellationToken);

    return Created(
            $"/projects/{projectSlug}/documentations/{documentation.Slug}",
            documentation);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectDocumentationsOutput), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetDocumentations(
      string projectSlug,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      [FromQuery] DocumentationKind? kind = null,
      [FromQuery] DocumentationStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectDocumentationsInput input = new(page, pageSize, query, kind, status);
    ListProjectDocumentationsOutput output =
        await listProjectDocumentationsUseCase.Execute(projectSlug, input, cancellationToken);

    return Ok(output);
  }

  [HttpGet("{slug}")]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetDocumentation(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    Documentation documentation = await getProjectDocumentationBySlugUseCase.Execute(projectSlug, slug, cancellationToken);
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
      [FromBody] UpdateProjectDocumentationInput input,
      CancellationToken cancellationToken = default)
  {
    Documentation documentation = await updateProjectDocumentationUseCase.Execute(projectSlug, slug, input, cancellationToken);
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
    await deleteProjectDocumentationUseCase.Execute(projectSlug, slug, cancellationToken);
    return NoContent();
  }
}
