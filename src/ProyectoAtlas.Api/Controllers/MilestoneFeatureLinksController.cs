using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Milestones;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/milestones/{milestoneSlug}/features")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class MilestoneFeatureLinksController(
    CreateMilestoneFeatureLinkCommandHandler createMilestoneFeatureLinkCommandHandler,
    ListMilestoneFeatureLinksQueryHandler listMilestoneFeatureLinksQueryHandler,
    DeleteMilestoneFeatureLinkCommandHandler deleteMilestoneFeatureLinkCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(MilestoneFeatureLink), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateLink(
      string projectSlug,
      string milestoneSlug,
      [FromBody] CreateMilestoneFeatureLinkCommand command,
      CancellationToken cancellationToken = default)
  {
    MilestoneFeatureLink link = await createMilestoneFeatureLinkCommandHandler.Execute(
        projectSlug,
        milestoneSlug,
        command,
        cancellationToken);

    return Created($"/projects/{projectSlug}/milestones/{milestoneSlug}/features/{link.Id}", link);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListMilestoneFeatureLinksResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetLinks(
      string projectSlug,
      string milestoneSlug,
      CancellationToken cancellationToken = default)
  {
    ListMilestoneFeatureLinksResponse response = await listMilestoneFeatureLinksQueryHandler.Execute(
        projectSlug,
        milestoneSlug,
        cancellationToken);

    return Ok(response);
  }

  [HttpDelete("{linkId:guid}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteLink(
      string projectSlug,
      string milestoneSlug,
      Guid linkId,
      CancellationToken cancellationToken = default)
  {
    await deleteMilestoneFeatureLinkCommandHandler.Execute(projectSlug, milestoneSlug, linkId, cancellationToken);
    return NoContent();
  }
}
