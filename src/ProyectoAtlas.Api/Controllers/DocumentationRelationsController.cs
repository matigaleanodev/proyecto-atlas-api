using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations/{sourceDocumentationSlug}/relations")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class DocumentationRelationsController(
    CreateDocumentationRelationCommandHandler createDocumentationRelationCommandHandler,
    ListDocumentationRelationsQueryHandler listDocumentationRelationsQueryHandler,
    DeleteDocumentationRelationCommandHandler deleteDocumentationRelationCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(DocumentationRelation), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateRelation(
      string projectSlug,
      string sourceDocumentationSlug,
      [FromBody] CreateDocumentationRelationCommand command,
      CancellationToken cancellationToken = default)
  {
    DocumentationRelation relation = await createDocumentationRelationCommandHandler.Execute(
        projectSlug,
        sourceDocumentationSlug,
        command,
        cancellationToken);

    return Created(
        $"/projects/{projectSlug}/documentations/{sourceDocumentationSlug}/relations/{relation.Id}",
        relation);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListDocumentationRelationsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetRelations(
      string projectSlug,
      string sourceDocumentationSlug,
      CancellationToken cancellationToken = default)
  {
    ListDocumentationRelationsResponse response = await listDocumentationRelationsQueryHandler.Execute(
        projectSlug,
        sourceDocumentationSlug,
        cancellationToken);

    return Ok(response);
  }

  [HttpDelete("{relationId:guid}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteRelation(
      string projectSlug,
      string sourceDocumentationSlug,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    await deleteDocumentationRelationCommandHandler.Execute(
        projectSlug,
        sourceDocumentationSlug,
        relationId,
        cancellationToken);

    return NoContent();
  }
}
