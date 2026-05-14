namespace ProyectoAtlas.Domain.Features;

public sealed class InvalidFeatureDocumentationLinkException(string message) : Exception(message);
