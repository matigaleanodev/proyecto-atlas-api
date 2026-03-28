namespace ProyectoAtlas.Domain.Documentations;

public sealed class InvalidDocumentationTagListException(string message) : Exception(message)
{
}
