using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/audit-events")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectAuditEventsController(ListProjectAuditEventsQueryHandler listProjectAuditEventsQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(ListProjectAuditEventsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAuditEvents(
      string projectSlug,
      CancellationToken cancellationToken = default)
  {
    ListProjectAuditEventsResponse response = await listProjectAuditEventsQueryHandler.Execute(
        projectSlug,
        new ListProjectAuditEventsQuery(),
        cancellationToken);

    return Ok(response);
  }
}
