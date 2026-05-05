using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/activity-feed")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectActivityFeedController(
    GetProjectActivityFeedQueryHandler getProjectActivityFeedQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(GetProjectActivityFeedResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetActivityFeed(
      string projectSlug,
      CancellationToken cancellationToken = default)
  {
    GetProjectActivityFeedResponse response = await getProjectActivityFeedQueryHandler.Execute(
        projectSlug,
        cancellationToken);

    return Ok(response);
  }
}
