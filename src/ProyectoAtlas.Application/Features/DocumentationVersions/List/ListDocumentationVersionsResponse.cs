using ProyectoAtlas.Domain.Documentations;

namespace ProyectoAtlas.Application.Features.DocumentationVersions.List;

public record ListDocumentationVersionsResponse(IReadOnlyCollection<DocumentationVersion> Items);
