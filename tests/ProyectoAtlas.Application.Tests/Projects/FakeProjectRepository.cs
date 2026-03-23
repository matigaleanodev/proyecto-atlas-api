using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Tests;

internal sealed class FakeProjectRepository : IProjectRepository
{
    public Project? AddedProject { get; private set; }
    public Project? UpdatedProject { get; private set; }
    public Project? DeletedProject { get; private set; }
    public int ReceivedPage { get; private set; }
    public int ReceivedPageSize { get; private set; }
    public string? ReceivedQuery { get; private set; }
    public IEnumerable<Project> PagedProjects { get; set; } = Enumerable.Empty<Project>();
    public int PagedTotalCount { get; set; }
    public Project? ProjectBySlug { get; set; }

    public Task Add(Project project, CancellationToken cancellationToken = default)
    {
        AddedProject = project;
        return Task.CompletedTask;
    }

    public Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedList(
        int page,
        int pageSize,
        string? query = null,
        CancellationToken cancellationToken = default)
    {
        ReceivedPage = page;
        ReceivedPageSize = pageSize;
        ReceivedQuery = query;

        return Task.FromResult((PagedProjects, PagedTotalCount));
    }

    public Task<Project?> GetBySlug(string slug, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ProjectBySlug);
    }

    public Task Update(Project project, CancellationToken cancellationToken = default)
    {
        UpdatedProject = project;
        return Task.CompletedTask;
    }

    public Task Delete(Project project, CancellationToken cancellationToken = default)
    {
        DeletedProject = project;
        return Task.CompletedTask;
    }
}
