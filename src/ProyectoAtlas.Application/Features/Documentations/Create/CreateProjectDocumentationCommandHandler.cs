
using ProyectoAtlas.Domain.Documentations;
using ProyectoAtlas.Domain.Projects;

namespace ProyectoAtlas.Application.Features.Documentations.Create;

public class CreateProjectDocumentationCommandHandler(
    IDocumentationRepository documentationRepository,
    IProjectRepository projectRepository)
{
  public async Task<Documentation> Execute(
      string projectSlug,
      CreateProjectDocumentationCommand input,
      CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.Title);
    ArgumentException.ThrowIfNullOrWhiteSpace(input.ContentMarkdown);

    if (input.Kind == DocumentationKind.FAQ)
    {
      if (input.FaqItems is null || input.FaqItems.Count == 0)
      {
        throw new InvalidDocumentationFaqItemsException("FAQ documentation must have at least one FAQ item.");
      }

      if (input.FaqItems.Any(item => string.IsNullOrWhiteSpace(item.Question)))
      {
        throw new InvalidDocumentationFaqItemsException("FAQ items must have a non-empty question.");
      }

      if (input.FaqItems.Any(item => string.IsNullOrWhiteSpace(item.Answer)))
      {
        throw new InvalidDocumentationFaqItemsException("FAQ items must have a non-empty answer.");
      }
    }

    if (input.Kind != DocumentationKind.FAQ && input.FaqItems is not null && input.FaqItems.Count > 0)
    {
      throw new InvalidDocumentationFaqItemsException("Only FAQ documentation can have FAQ items.");
    }

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

    Project project = await projectRepository.GetBySlug(projectSlug, cancellationToken)
        ?? throw new ProjectNotFoundException(projectSlug);

    Documentation documentation;

    try
    {

      List<DocumentationFaqItemData>? faqItems = input.FaqItems?
        .Select(item => new DocumentationFaqItemData(
            item.Question,
            item.Answer,
            item.SortOrder))
        .ToList();

      List<DocumentationTagData>? tags = input.Tags?
        .Select(tag => new DocumentationTagData(tag.Name))
        .ToList();

      documentation = new(
        projectId: project.Id,
        title: input.Title,
        contentMarkdown: input.ContentMarkdown,
        sortOrder: input.SortOrder,
        kind: input.Kind,
        status: input.Status,
        area: input.Area,
        tags: tags,
        faqItems: faqItems);

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

    await documentationRepository.Add(documentation, cancellationToken);

    return documentation;
  }
}
