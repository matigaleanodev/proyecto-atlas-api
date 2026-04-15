namespace ProyectoAtlas.Domain.Projects;

public sealed class InvalidProjectRelationException(string message) : Exception(message);
