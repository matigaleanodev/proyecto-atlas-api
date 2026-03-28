using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Documentations.Update;

public class UpdateProjectDocumentationCommandHandler(IDocumentationRepository documentationRepository, IProjectRepository projectRepository)
{
  public async Task<Documentation> Execute(string projectSlug, string slug, UpdateProjectDocumentationCommand input, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation = await documentationRepository.GetBySlug(project.Id, slug, cancellationToken)
        ?? throw new DocumentationNotFoundException(projectSlug, slug);

    try
    {
      documentation.Update(
        title: input.Title,
        contentMarkdown: input.ContentMarkdown,
        sortOrder: input.SortOrder,
        status: input.Status
      );

      if (input.FaqItems is not null)
      {
        IReadOnlyCollection<DocumentationFaqItemData> faqItems = input.FaqItems
            .Select(item => new DocumentationFaqItemData(
                item.Question,
                item.Answer,
                item.SortOrder))
            .ToList();

        documentation.ReplaceFaqItems(faqItems);
      }
    }
    catch (InvalidDocumentationTitleException exception)
    {
      throw new InvalidDocumentationTitleConventionException(exception.Message);
    }
    catch (InvalidDocumentationFaqListException exception)
    {
      throw new InvalidDocumentationFaqItemsException(exception.Message);
    }

    await documentationRepository.Update(documentation, cancellationToken);

    return documentation;
  }
}
