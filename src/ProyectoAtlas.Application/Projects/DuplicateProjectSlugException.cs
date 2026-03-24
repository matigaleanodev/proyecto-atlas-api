namespace ProyectoAtlas.Application.Projects;

public class DuplicateProjectSlugException(string slug)
    : Exception($"Project slug '{slug}' already exists.");
