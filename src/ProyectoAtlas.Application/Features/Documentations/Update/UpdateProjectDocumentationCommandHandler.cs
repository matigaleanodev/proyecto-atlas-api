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

    if (input.Tags is not null)
    {
      if (input.Tags.Any(tag => string.IsNullOrWhiteSpace(tag.Name)))
      {
        throw new InvalidDocumentationTagsException("Documentation tags must have a non-empty name.");
      }

      int distinctTagsCount = input.Tags
          .Select(tag => tag.Name.Trim().ToLowerInvariant())
          .Distinct()
          .Count();

      if (distinctTagsCount != input.Tags.Count)
      {
        throw new InvalidDocumentationTagsException("Documentation cannot have duplicate tags.");
      }
    }

    if (input.FaqItems is not null && input.FaqItems.Any(item => item.SortOrder < 1))
    {
      throw new InvalidDocumentationFaqItemsException("FAQ items must have a sort order greater than 0.");
    }

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

      if (input.Tags is not null)
      {
        IReadOnlyCollection<DocumentationTagData> tags = input.Tags
            .Select(tag => new DocumentationTagData(tag.Name))
            .ToList();

        documentation.ReplaceTags(tags);
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
    catch (InvalidDocumentationTagListException exception)
    {
      throw new InvalidDocumentationTagsException(exception.Message);
    }

    await documentationRepository.Update(documentation, cancellationToken);

    return documentation;
  }
}
