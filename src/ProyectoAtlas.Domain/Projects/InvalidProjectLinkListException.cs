namespace ProyectoAtlas.Domain.Projects;

public sealed class InvalidProjectLinkListException(string message) : Exception(message)
{
}
