using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Application.Projects.CreateProject;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects")]
public class ProjectsController(CreateProjectUseCase createProjectUseCase) : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> CreateProject(
      [FromBody] CreateProjectInput input,
      CancellationToken cancellationToken)
  {
    var project = await createProjectUseCase.Execute(input, cancellationToken);

    return Created($"/projects/{project.Id}", project);
  }
}
