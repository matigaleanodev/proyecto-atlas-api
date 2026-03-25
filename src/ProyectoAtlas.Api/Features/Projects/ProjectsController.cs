using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.Features.Projects;

[ApiController]
[Route("projects")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectsController(
    CreateProjectCommandHandler createProjectCommandHandler,
    ListProjectsQueryHandler listProjectsQueryHandler,
    GetProjectBySlugQueryHandler getProjectBySlugQueryHandler,
    UpdateProjectCommandHandler updateProjectCommandHandler,
    DeleteProjectCommandHandler deleteProjectCommandHandler
  ) : ControllerBase
{

  [HttpPost]
  [ProducesResponseType(typeof(Project), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateProject(
      [FromBody] CreateProjectCommand command,
      CancellationToken cancellationToken)
  {
    Project project = await createProjectCommandHandler.Execute(command, cancellationToken);

    return Created($"/projects/{project.Id}", project);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectsResponse), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetProjects(
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectsQuery queryModel = new(page, pageSize, query);

    ListProjectsResponse response = await listProjectsQueryHandler.Execute(queryModel, cancellationToken);

    return Ok(response);
  }

  [HttpGet("{slug}")]
  [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProjectBySlug(
      string slug,
      CancellationToken cancellationToken = default)
  {
    Project project = await getProjectBySlugQueryHandler.Execute(slug, cancellationToken);
    return Ok(project);
  }

  [HttpPatch("{slug}")]
  [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> UpdateProject(
      string slug,
      [FromBody] UpdateProjectCommand command,
      CancellationToken cancellationToken = default)
  {
    Project project = await updateProjectCommandHandler.Execute(slug, command, cancellationToken);
    return Ok(project);
  }

  [HttpDelete("{slug}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteProject(
      string slug,
      CancellationToken cancellationToken = default)
  {
    await deleteProjectCommandHandler.Execute(slug, cancellationToken);
    return NoContent();
  }
}
