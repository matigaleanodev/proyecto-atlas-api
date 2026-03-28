
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

      documentation = new(
        project.Id,
        input.Title,
        input.ContentMarkdown,
        input.SortOrder,
        input.Kind,
        input.Status,
        input.Area,
        faqItems);

    }
    catch (InvalidDocumentationTitleException exception)
    {
      throw new InvalidDocumentationTitleConventionException(exception.Message);
    }
    catch (InvalidDocumentationFaqListException exception)
    {
      throw new InvalidDocumentationFaqItemsException(exception.Message);
    }

    await documentationRepository.Add(documentation, cancellationToken);

    return documentation;
  }
}
