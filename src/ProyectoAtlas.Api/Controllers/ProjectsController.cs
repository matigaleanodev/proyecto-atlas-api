using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectsController(
    CreateProjectUseCase createProjectUseCase,
    ListProjectsUseCase listProjectsUseCase,
    GetProjectBySlugUseCase getProjectBySlugUseCase,
    UpdateProjectUseCase updateProjectUseCase,
    DeleteProjectUseCase deleteProjectUseCase
  ) : ControllerBase
{

  [HttpPost]
  [ProducesResponseType(typeof(Project), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateProject(
      [FromBody] CreateProjectInput input,
      CancellationToken cancellationToken)
  {
    Project project = await createProjectUseCase.Execute(input, cancellationToken);

    return Created($"/projects/{project.Id}", project);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectsOutput), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetProjects(
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectsInput input = new(page, pageSize, query);

    ListProjectsOutput output = await listProjectsUseCase.Execute(input, cancellationToken);

    return Ok(output);
  }

  [HttpGet("{slug}")]
  [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProjectBySlug(
      string slug,
      CancellationToken cancellationToken = default)
  {
    Project project = await getProjectBySlugUseCase.Execute(slug, cancellationToken);
    return Ok(project);
  }

  [HttpPatch("{slug}")]
  [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> UpdateProject(
      string slug,
      [FromBody] UpdateProjectInput input,
      CancellationToken cancellationToken = default)
  {
    Project project = await updateProjectUseCase.Execute(slug, input, cancellationToken);
    return Ok(project);
  }

  [HttpDelete("{slug}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteProject(
      string slug,
      CancellationToken cancellationToken = default)
  {
    await deleteProjectUseCase.Execute(slug, cancellationToken);
    return NoContent();
  }
}
