using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/features/{featureSlug}/documentations")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class FeatureDocumentationLinksController(
    CreateFeatureDocumentationLinkCommandHandler createFeatureDocumentationLinkCommandHandler,
    ListFeatureDocumentationLinksQueryHandler listFeatureDocumentationLinksQueryHandler,
    DeleteFeatureDocumentationLinkCommandHandler deleteFeatureDocumentationLinkCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(FeatureDocumentationLink), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateLink(
      string projectSlug,
      string featureSlug,
      [FromBody] CreateFeatureDocumentationLinkCommand command,
      CancellationToken cancellationToken = default)
  {
    FeatureDocumentationLink link = await createFeatureDocumentationLinkCommandHandler.Execute(
        projectSlug,
        featureSlug,
        command,
        cancellationToken);

    return Created($"/projects/{projectSlug}/features/{featureSlug}/documentations/{link.Id}", link);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListFeatureDocumentationLinksResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetLinks(
      string projectSlug,
      string featureSlug,
      CancellationToken cancellationToken = default)
  {
    ListFeatureDocumentationLinksResponse response = await listFeatureDocumentationLinksQueryHandler.Execute(
        projectSlug,
        featureSlug,
        cancellationToken);

    return Ok(response);
  }

  [HttpDelete("{linkId:guid}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteLink(
      string projectSlug,
      Guid linkId,
      CancellationToken cancellationToken = default)
  {
    await deleteFeatureDocumentationLinkCommandHandler.Execute(projectSlug, linkId, cancellationToken);
    return NoContent();
  }
}
