using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects")]
public class ProjectsController(
    CreateProjectUseCase createProjectUseCase,
    ListProjectsUseCase listProjectsUseCase,
    GetProjectBySlugUseCase getProjectBySlugUseCase,
    UpdateProjectUseCase updateProjectUseCase,
    DeleteProjectUseCase deleteProjectUseCase
  ) : ControllerBase
{

  [HttpGet]
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
  public async Task<IActionResult> GetProjectBySlug(
      string slug,
      CancellationToken cancellationToken = default)
  {
    try
    {
      Project project = await getProjectBySlugUseCase.Execute(slug, cancellationToken);
      return Ok(project);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  [HttpPatch("{slug}")]
  public async Task<IActionResult> UpdateProject(
      string slug,
      [FromBody] UpdateProjectInput input,
      CancellationToken cancellationToken = default)
  {
    try
    {
      Project project = await updateProjectUseCase.Execute(slug, input, cancellationToken);
      return Ok(project);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  [HttpDelete("{slug}")]
  public async Task<IActionResult> DeleteProject(
      string slug,
      CancellationToken cancellationToken = default)
  {
    try
    {
      Project project = await deleteProjectUseCase.Execute(slug, cancellationToken);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  [HttpPost]
  public async Task<IActionResult> CreateProject(
      [FromBody] CreateProjectInput input,
      CancellationToken cancellationToken)
  {
    Project project = await createProjectUseCase.Execute(input, cancellationToken);

    return Created($"/projects/{project.Id}", project);
  }
}
