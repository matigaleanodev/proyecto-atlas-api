using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/features/{featureSlug}/milestones")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class FeatureMilestoneLinksController(
    ListFeatureMilestoneLinksQueryHandler listFeatureMilestoneLinksQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(ListFeatureMilestoneLinksResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetLinks(
      string projectSlug,
      string featureSlug,
      CancellationToken cancellationToken = default)
  {
    ListFeatureMilestoneLinksResponse response = await listFeatureMilestoneLinksQueryHandler.Execute(
        projectSlug,
        featureSlug,
        cancellationToken);

    return Ok(response);
  }
}
