using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Domain.Audit;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations/{documentationSlug}/audit-events")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class DocumentationAuditEventsController(ListDocumentationAuditEventsQueryHandler listDocumentationAuditEventsQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(ListDocumentationAuditEventsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAuditEvents(
      string projectSlug,
      string documentationSlug,
      [FromQuery] AuditEntityType? entityType = null,
      [FromQuery] AuditAction? action = null,
      [FromQuery] DateTime? occurredFromUtc = null,
      [FromQuery] DateTime? occurredToUtc = null,
      [FromQuery] int? limit = null,
      CancellationToken cancellationToken = default)
  {
    ListDocumentationAuditEventsResponse response = await listDocumentationAuditEventsQueryHandler.Execute(
        projectSlug,
        documentationSlug,
        new ListDocumentationAuditEventsQuery(entityType, action, occurredFromUtc, occurredToUtc, limit),
        cancellationToken);

    return Ok(response);
  }
}
