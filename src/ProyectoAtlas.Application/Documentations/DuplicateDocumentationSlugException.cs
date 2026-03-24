namespace ProyectoAtlas.Application.Documentations;

public class DuplicateDocumentationSlugException(string slug)
    : Exception($"Documentation slug '{slug}' already exists for the current project.");
