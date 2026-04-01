using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Features;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/features")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectFeaturesController(
  CreateProjectFeatureCommandHandler createProjectFeatureCommandHandler,
  ListProjectFeaturesQueryHandler listProjectFeaturesQueryHandler,
  GetProjectFeatureBySlugQueryHandler getProjectFeatureBySlugQueryHandler,
  UpdateProjectFeatureCommandHandler updateProjectFeatureCommandHandler,
  DeleteProjectFeatureCommandHandler deleteProjectFeatureCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(Feature), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateFeature(
      string projectSlug,
      [FromBody] CreateProjectFeatureCommand command,
      CancellationToken cancellationToken = default)
  {
    Feature feature = await createProjectFeatureCommandHandler.Execute(projectSlug, command, cancellationToken);

    return Created($"/projects/{projectSlug}/features/{feature.Slug}", feature);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectFeaturesResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetFeatures(
      string projectSlug,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      [FromQuery] FeatureStatus? status = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectFeaturesQuery queryModel = new(page, pageSize, query, status);
    ListProjectFeaturesResponse response = await listProjectFeaturesQueryHandler.Execute(projectSlug, queryModel, cancellationToken);

    return Ok(response);
  }

  [HttpGet("{slug}")]
  [ProducesResponseType(typeof(Feature), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetFeature(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    Feature feature = await getProjectFeatureBySlugQueryHandler.Execute(projectSlug, slug, cancellationToken);
    return Ok(feature);
  }

  [HttpPatch("{slug}")]
  [ProducesResponseType(typeof(Feature), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> UpdateFeature(
      string projectSlug,
      string slug,
      [FromBody] UpdateProjectFeatureCommand command,
      CancellationToken cancellationToken = default)
  {
    Feature feature = await updateProjectFeatureCommandHandler.Execute(projectSlug, slug, command, cancellationToken);
    return Ok(feature);
  }

  [HttpDelete("{slug}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteFeature(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    await deleteProjectFeatureCommandHandler.Execute(projectSlug, slug, cancellationToken);
    return NoContent();
  }
}
