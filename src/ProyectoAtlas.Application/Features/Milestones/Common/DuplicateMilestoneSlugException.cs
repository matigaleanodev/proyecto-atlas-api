using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.Milestones.Common;

public sealed class DuplicateMilestoneSlugException(string slug)
    : KnownException(
        $"Milestone slug '{slug}' already exists.",
        AtlasErrorCodes.MilestoneSlugConflict,
        HttpStatusCode.Conflict);
