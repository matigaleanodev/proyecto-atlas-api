using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations/{documentationSlug}/resources")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class DocumentationResourcesController(
    CreateDocumentationResourceCommandHandler createDocumentationResourceCommandHandler,
    ListDocumentationResourcesQueryHandler listDocumentationResourcesQueryHandler,
    DeleteDocumentationResourceCommandHandler deleteDocumentationResourceCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(DocumentationResource), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateResource(
      string projectSlug,
      string documentationSlug,
      [FromBody] CreateDocumentationResourceCommand command,
      CancellationToken cancellationToken = default)
  {
    DocumentationResource resource = await createDocumentationResourceCommandHandler.Execute(
        projectSlug,
        documentationSlug,
        command,
        cancellationToken);

    return Created(
        $"/projects/{projectSlug}/documentations/{documentationSlug}/resources/{resource.Id}",
        resource);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListDocumentationResourcesResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetResources(
      string projectSlug,
      string documentationSlug,
      [FromQuery] DocumentationResourceKind? kind = null,
      CancellationToken cancellationToken = default)
  {
    ListDocumentationResourcesQuery query = new(kind);
    ListDocumentationResourcesResponse response = await listDocumentationResourcesQueryHandler.Execute(
        projectSlug,
        documentationSlug,
        query,
        cancellationToken);

    return Ok(response);
  }

  [HttpDelete("{resourceId:guid}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteResource(
      string projectSlug,
      string documentationSlug,
      Guid resourceId,
      CancellationToken cancellationToken = default)
  {
    await deleteDocumentationResourceCommandHandler.Execute(
        projectSlug,
        documentationSlug,
        resourceId,
        cancellationToken);

    return NoContent();
  }
}
