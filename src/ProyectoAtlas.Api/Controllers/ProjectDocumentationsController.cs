using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations")]
public class ProjectDocumentationsController(
  CreateProjectDocumentationUseCase createDocumentationUseCase,
  ListProjectDocumentationsUseCase listProjectDocumentationsUseCase,
  GetProjectDocumentationBySlugUseCase getProjectDocumentationBySlugUseCase,
  UpdateProjectDocumentationUseCase updateProjectDocumentationUseCase,
  DeleteProjectDocumentationUseCase deleteProjectDocumentationUseCase
  ) : ControllerBase
{

  [HttpPost]
  public async Task<IActionResult> CreateDocumentation(
      string projectSlug,
      [FromBody] CreateProjectDocumentationInput input,
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

  [HttpGet("{slug}")]
  public async Task<IActionResult> GetDocumentation(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    try
    {
      Documentation documentation = await getProjectDocumentationBySlugUseCase.Execute(projectSlug, slug, cancellationToken);
      return Ok(documentation);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  [HttpPatch("{slug}")]
  public async Task<IActionResult> UpdateDocumentation(
      string projectSlug,
      string slug,
      [FromBody] UpdateProjectDocumentationInput input,
      CancellationToken cancellationToken = default)
  {
    try
    {
      Documentation documentation = await updateProjectDocumentationUseCase.Execute(projectSlug, slug, input, cancellationToken);
      return Ok(documentation);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  [HttpDelete("{slug}")]
  public async Task<IActionResult> DeleteDocumentation(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    try
    {
      await deleteProjectDocumentationUseCase.Execute(projectSlug, slug, cancellationToken);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }

  }
}
