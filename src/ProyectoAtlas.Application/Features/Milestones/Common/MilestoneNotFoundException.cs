using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Milestones.Common;

public sealed class MilestoneNotFoundException(string projectSlug, string slug)
    : KnownException(
        $"Milestone with slug '{slug}' was not found in project '{projectSlug}'.",
        AtlasErrorCodes.MilestoneNotFound,
        HttpStatusCode.NotFound);
