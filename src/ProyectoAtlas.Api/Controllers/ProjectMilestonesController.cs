using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/milestones")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectMilestonesController(
    CreateProjectMilestoneCommandHandler createProjectMilestoneCommandHandler,
    ListProjectMilestonesQueryHandler listProjectMilestonesQueryHandler,
    GetProjectMilestoneBySlugQueryHandler getProjectMilestoneBySlugQueryHandler,
    UpdateProjectMilestoneCommandHandler updateProjectMilestoneCommandHandler,
    DeleteProjectMilestoneCommandHandler deleteProjectMilestoneCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(Milestone), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateMilestone(
      string projectSlug,
      [FromBody] CreateProjectMilestoneCommand command,
      CancellationToken cancellationToken = default)
  {
    Milestone milestone = await createProjectMilestoneCommandHandler.Execute(projectSlug, command, cancellationToken);

    return Created($"/projects/{projectSlug}/milestones/{milestone.Slug}", milestone);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectMilestonesResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetMilestones(
      string projectSlug,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      [FromQuery] MilestoneStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectMilestonesQuery queryModel = new(page, pageSize, query, status);
    ListProjectMilestonesResponse response = await listProjectMilestonesQueryHandler.Execute(projectSlug, queryModel, cancellationToken);

    return Ok(response);
  }

  [HttpGet("{slug}")]
  [ProducesResponseType(typeof(Milestone), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetMilestone(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    Milestone milestone = await getProjectMilestoneBySlugQueryHandler.Execute(projectSlug, slug, cancellationToken);
    return Ok(milestone);
  }

  [HttpPatch("{slug}")]
  [ProducesResponseType(typeof(Milestone), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> UpdateMilestone(
      string projectSlug,
      string slug,
      [FromBody] UpdateProjectMilestoneCommand command,
      CancellationToken cancellationToken = default)
  {
    Milestone milestone = await updateProjectMilestoneCommandHandler.Execute(projectSlug, slug, command, cancellationToken);
    return Ok(milestone);
  }

  [HttpDelete("{slug}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteMilestone(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    await deleteProjectMilestoneCommandHandler.Execute(projectSlug, slug, cancellationToken);
    return NoContent();
  }
}
