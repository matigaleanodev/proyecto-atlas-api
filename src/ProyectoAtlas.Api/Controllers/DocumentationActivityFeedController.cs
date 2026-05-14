using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations/{documentationSlug}/activity-feed")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class DocumentationActivityFeedController(
    GetDocumentationActivityFeedQueryHandler getDocumentationActivityFeedQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(GetDocumentationActivityFeedResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetActivityFeed(
      string projectSlug,
      string documentationSlug,
      CancellationToken cancellationToken = default)
  {
    GetDocumentationActivityFeedResponse response = await getDocumentationActivityFeedQueryHandler.Execute(
        projectSlug,
        documentationSlug,
        cancellationToken);

    return Ok(response);
  }
}
