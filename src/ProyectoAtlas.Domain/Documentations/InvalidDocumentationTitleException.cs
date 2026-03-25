namespace ProyectoAtlas.Domain.Documentations;

public sealed class InvalidDocumentationTitleException(string message) : Exception(message)
{
}
