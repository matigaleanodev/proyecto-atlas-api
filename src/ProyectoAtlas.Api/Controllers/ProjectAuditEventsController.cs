using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/audit-events")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProjectAuditEventsController(ListProjectAuditEventsQueryHandler listProjectAuditEventsQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(ListProjectAuditEventsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAuditEvents(
      string projectSlug,
      [FromQuery] AuditEntityType? entityType = null,
      [FromQuery] AuditAction? action = null,
      [FromQuery] DateTime? occurredFromUtc = null,
      [FromQuery] DateTime? occurredToUtc = null,
      [FromQuery] int? limit = null,
      CancellationToken cancellationToken = default)
  {
    ListProjectAuditEventsResponse response = await listProjectAuditEventsQueryHandler.Execute(
        projectSlug,
        new ListProjectAuditEventsQuery(entityType, action, occurredFromUtc, occurredToUtc, limit),
        cancellationToken);

    return Ok(response);
  }
}
