using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations/{documentationSlug}/features")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class DocumentationFeatureLinksController(
    ListDocumentationFeatureLinksQueryHandler listDocumentationFeatureLinksQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(ListDocumentationFeatureLinksResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetLinks(
      string projectSlug,
      string documentationSlug,
      CancellationToken cancellationToken = default)
  {
    ListDocumentationFeatureLinksResponse response = await listDocumentationFeatureLinksQueryHandler.Execute(
        projectSlug,
        documentationSlug,
        cancellationToken);

    return Ok(response);
  }
}
