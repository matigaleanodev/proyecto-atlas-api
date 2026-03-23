using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Application.Projects;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects")]
public class ProjectsController(CreateProjectUseCase createProjectUseCase, ListProjectsUseCase listProjectsUseCase) : ControllerBase
{

  [HttpGet]
  public async Task<IActionResult> GetProjects(
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      CancellationToken cancellationToken = default)
  {
    var input = new ListProjectsInput(page, pageSize, query);

    var output = await listProjectsUseCase.Execute(input, cancellationToken);

    return Ok(output);
  }

  [HttpPost]
  public async Task<IActionResult> CreateProject(
      [FromBody] CreateProjectInput input,
      CancellationToken cancellationToken)
  {
    var project = await createProjectUseCase.Execute(input, cancellationToken);

    return Created($"/projects/{project.Id}", project);
  }
}
