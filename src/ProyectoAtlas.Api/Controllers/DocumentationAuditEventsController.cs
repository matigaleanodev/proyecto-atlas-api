using Microsoft.AspNetCore.Mvc;
using ProyectoAtlas.Api.Errors;

namespace ProyectoAtlas.Api.Controllers;

[ApiController]
[Route("projects/{projectSlug}/documentations/{documentationSlug}/audit-events")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
public class DocumentationAuditEventsController(ListDocumentationAuditEventsQueryHandler listDocumentationAuditEventsQueryHandler) : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(ListDocumentationAuditEventsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAuditEvents(
      string projectSlug,
      string documentationSlug,
      CancellationToken cancellationToken = default)
  {
    ListDocumentationAuditEventsResponse response = await listDocumentationAuditEventsQueryHandler.Execute(
        projectSlug,
        documentationSlug,
        new ListDocumentationAuditEventsQuery(),
        cancellationToken);

    return Ok(response);
  }
}
