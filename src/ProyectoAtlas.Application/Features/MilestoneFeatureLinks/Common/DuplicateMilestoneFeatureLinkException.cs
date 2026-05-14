using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.Common;

public sealed class DuplicateMilestoneFeatureLinkException()
    : KnownException(
        "Milestone feature link already exists for the current project.",
        AtlasErrorCodes.MilestoneFeatureLinkConflict,
        HttpStatusCode.Conflict);
