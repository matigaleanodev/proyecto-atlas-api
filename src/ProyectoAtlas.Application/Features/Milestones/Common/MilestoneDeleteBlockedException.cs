using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Milestones.Common;

public sealed class MilestoneDeleteBlockedException(string projectSlug, string slug, string reason)
    : KnownException(
        $"Milestone with slug '{slug}' cannot be deleted for project '{projectSlug}' because {reason}.",
        AtlasErrorCodes.MilestoneDeleteBlocked,
        HttpStatusCode.Conflict);
