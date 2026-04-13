using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{sourceProjectSlug}/relations")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectRelationsController(
    CreateProjectRelationCommandHandler createProjectRelationCommandHandler,
    ListProjectRelationsQueryHandler listProjectRelationsQueryHandler,
    DeleteProjectRelationCommandHandler deleteProjectRelationCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(ProjectRelation), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateRelation(
      string sourceProjectSlug,
      [FromBody] CreateProjectRelationCommand command,
      CancellationToken cancellationToken = default)
  {
    ProjectRelation relation = await createProjectRelationCommandHandler.Execute(
        sourceProjectSlug,
        command,
        cancellationToken);

    return Created(
        $"/projects/{sourceProjectSlug}/relations/{relation.Id}",
        relation);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectRelationsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetRelations(
      string sourceProjectSlug,
      CancellationToken cancellationToken = default)
  {
    ListProjectRelationsResponse response = await listProjectRelationsQueryHandler.Execute(
        sourceProjectSlug,
        cancellationToken);

    return Ok(response);
  }

  [HttpDelete("{relationId:guid}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteRelation(
      string sourceProjectSlug,
      Guid relationId,
      CancellationToken cancellationToken = default)
  {
    await deleteProjectRelationCommandHandler.Execute(
        sourceProjectSlug,
        relationId,
        cancellationToken);

    return NoContent();
  }
}
