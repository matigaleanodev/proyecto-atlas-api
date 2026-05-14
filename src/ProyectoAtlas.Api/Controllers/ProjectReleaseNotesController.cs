using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/release-notes")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectReleaseNotesController(
    CreateProjectReleaseNotesCommandHandler createProjectReleaseNotesCommandHandler,
    ListProjectReleaseNotesQueryHandler listProjectReleaseNotesQueryHandler,
    GetProjectReleaseNotesBySlugQueryHandler getProjectReleaseNotesBySlugQueryHandler,
    UpdateProjectReleaseNotesCommandHandler updateProjectReleaseNotesCommandHandler,
    DeleteProjectReleaseNotesCommandHandler deleteProjectReleaseNotesCommandHandler) : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateReleaseNotes(
      string projectSlug,
      [FromBody] CreateProjectReleaseNotesCommand command,
      CancellationToken cancellationToken = default)
  {
    Documentation releaseNotes = await createProjectReleaseNotesCommandHandler.Execute(projectSlug, command, cancellationToken);

    return Created($"/projects/{projectSlug}/release-notes/{releaseNotes.Slug}", releaseNotes);
  }

  [HttpGet]
  [ProducesResponseType(typeof(ListProjectDocumentationsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetReleaseNotes(
      string projectSlug,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? query = null,
      [FromQuery] DocumentationStatus? status = null,
      [FromQuery] DocumentationArea? area = null,
      [FromQuery] string? tag = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectReleaseNotesQuery queryModel = new(page, pageSize, query, status, area, tag);
    ListProjectDocumentationsResponse response =
        await listProjectReleaseNotesQueryHandler.Execute(projectSlug, queryModel, cancellationToken);

    return Ok(response);
  }

  [HttpGet("{slug}")]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetReleaseNotesBySlug(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    Documentation releaseNotes = await getProjectReleaseNotesBySlugQueryHandler.Execute(projectSlug, slug, cancellationToken);
    return Ok(releaseNotes);
  }

  [HttpPatch("{slug}")]
  [ProducesResponseType(typeof(Documentation), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> UpdateReleaseNotes(
      string projectSlug,
      string slug,
      [FromBody] UpdateProjectReleaseNotesCommand command,
      CancellationToken cancellationToken = default)
  {
    Documentation releaseNotes =
        await updateProjectReleaseNotesCommandHandler.Execute(projectSlug, slug, command, cancellationToken);

    return Ok(releaseNotes);
  }

  [HttpDelete("{slug}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> DeleteReleaseNotes(
      string projectSlug,
      string slug,
      CancellationToken cancellationToken = default)
  {
    await deleteProjectReleaseNotesCommandHandler.Execute(projectSlug, slug, cancellationToken);
    return NoContent();
  }
}
