using System.Net;
using ProyectoAtlas.Application.Errors;

namespace ProyectoAtlas.Application.Features.MilestoneFeatureLinks.Common;

public sealed class MilestoneFeatureLinkNotFoundException(Guid linkId)
    : KnownException(
        $"Milestone feature link '{linkId}' was not found.",
        AtlasErrorCodes.MilestoneFeatureLinkNotFound,
        HttpStatusCode.NotFound);
