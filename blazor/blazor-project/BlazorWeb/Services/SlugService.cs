using System.Text;

namespace BlazorWeb.Services;

public sealed class SlugService
{
    public string CreateSlug(string? sSource)
    {
        if (string.IsNullOrWhiteSpace(sSource))
        {
            return Guid.NewGuid().ToString("N")[..12];
        }

        string sNormalizedSource = sSource.Trim().Normalize(NormalizationForm.FormC);
        StringBuilder stringBuilderSlug = new();
        bool bPreviousDash = false;

        for (int iIndex = 0; iIndex < sNormalizedSource.Length; iIndex++)
        {
            char cCurrent = sNormalizedSource[iIndex];
            char cLower = char.ToLowerInvariant(cCurrent);

            if (char.IsLetterOrDigit(cLower))
            {
                stringBuilderSlug.Append(cLower);
                bPreviousDash = false;
                continue;
            }

            if (!bPreviousDash)
            {
                stringBuilderSlug.Append('-');
                bPreviousDash = true;
            }
        }

        string sSlug = stringBuilderSlug.ToString().Trim('-');

        if (string.IsNullOrWhiteSpace(sSlug))
        {
            return Guid.NewGuid().ToString("N")[..12];
        }

        if (sSlug.Length > 160)
        {
            sSlug = sSlug[..160].Trim('-');
        }

        return sSlug;
    }
}
