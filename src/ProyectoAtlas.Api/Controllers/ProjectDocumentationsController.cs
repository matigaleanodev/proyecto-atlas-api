using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations")]
public class ProjectDocumentationsController(
  CreateDocumentationUseCase createDocumentationUseCase
  ) : ControllerBase
{

  [HttpPost]
  public async Task<IActionResult> CreateDocumentation(
      string projectSlug,
      [FromBody] CreateDocumentationInput input,
      CancellationToken cancellationToken = default)
  {
    try
    {
      Documentation documentation = await createDocumentationUseCase.Execute(projectSlug, input, cancellationToken);
      return Created(
              $"/projects/{projectSlug}/documentations/{documentation.Slug}",
              documentation);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

}
