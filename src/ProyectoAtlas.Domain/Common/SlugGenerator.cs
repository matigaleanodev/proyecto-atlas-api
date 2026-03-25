using System.Globalization;
using System.Text;

namespace ProyectoAtlas.Domain.Common;

public static class SlugGenerator
{
  public static string Generate(string value)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(value);

    string normalizedValue = value.Trim().Normalize(NormalizationForm.FormD);
    StringBuilder builder = new();
    bool previousWasSeparator = false;

    foreach (char character in normalizedValue)
    {
      UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);

      if (category == UnicodeCategory.NonSpacingMark)
      {
        continue;
      }

      if (char.IsLetterOrDigit(character))
      {
        builder.Append(char.ToLowerInvariant(character));
        previousWasSeparator = false;
        continue;
      }

      if (!previousWasSeparator && builder.Length > 0)
      {
        builder.Append('-');
        previousWasSeparator = true;
      }
    }

    return builder.ToString().Trim('-');
  }
}
