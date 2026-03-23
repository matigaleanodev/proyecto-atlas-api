using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations")]
public class ProjectDocumentationsController(
  CreateDocumentationUseCase createDocumentationUseCase,
  ListProjectDocumentationsUseCase listProjectDocumentationsUseCase
  ) : ControllerBase
{

  [HttpGet]
  public async Task<IActionResult> GetDocumentations(
      string projectSlug,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      CancellationToken cancellationToken = default)
  {
    try
    {
      ListProjectDocumentationsInput input = new(page, pageSize, query);
      ListProjectDocumentationsOutput output =
          await listProjectDocumentationsUseCase.Execute(projectSlug, input, cancellationToken);

      return Ok(output);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }


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
