using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations/{slug}/versions")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class DocumentationVersionsController(
    ListDocumentationVersionsQueryHandler listDocumentationVersionsQueryHandler,
    GetDocumentationVersionByNumberQueryHandler getDocumentationVersionByNumberQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(ListDocumentationVersionsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetVersions(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    ListDocumentationVersionsResponse response = await listDocumentationVersionsQueryHandler.Execute(
        projectSlug,
        slug,
        cancellationToken);

    return Ok(response);
  }

  [HttpGet("{versionNumber:int}")]
  [ProducesResponseType(typeof(DocumentationVersion), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetVersionByNumber(
      string projectSlug,
      string slug,
      int versionNumber,
      CancellationToken cancellationToken = default)
  {
    DocumentationVersion version = await getDocumentationVersionByNumberQueryHandler.Execute(
        projectSlug,
        slug,
        versionNumber,
        cancellationToken);

    return Ok(version);
  }
}
