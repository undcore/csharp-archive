using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace BlazorWeb.Services;

public sealed partial class MarkdownRenderService(SlugService slugService)
{
    public RenderedArticle Render(string? sMarkdown)
    {
        string sSource = sMarkdown ?? "";
        string[] arrLines = sSource.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        StringBuilder stringBuilderHtml = new();
        StringBuilder stringBuilderParagraph = new();
        StringBuilder stringBuilderCode = new();
        List<TableOfContentsItem> listTableOfContentsItems = [];
        Dictionary<string, int> dictionaryAnchorCounts = [];
        bool bInsideCodeBlock = false;
        string sCodeLanguage = "";

        for (int iIndex = 0; iIndex < arrLines.Length; iIndex++)
        {
            string sLine = arrLines[iIndex];

            if (sLine.StartsWith("```", StringComparison.Ordinal))
            {
                if (bInsideCodeBlock)
                {
                    AppendCodeBlock(stringBuilderHtml, sCodeLanguage, stringBuilderCode.ToString());
                    stringBuilderCode.Clear();
                    sCodeLanguage = "";
                    bInsideCodeBlock = false;
                    continue;
                }

                FlushParagraph(stringBuilderHtml, stringBuilderParagraph);
                sCodeLanguage = sLine[3..].Trim();
                bInsideCodeBlock = true;
                continue;
            }

            if (bInsideCodeBlock)
            {
                stringBuilderCode.AppendLine(sLine);
                continue;
            }

            if (string.IsNullOrWhiteSpace(sLine))
            {
                FlushParagraph(stringBuilderHtml, stringBuilderParagraph);
                continue;
            }

            int iHeadingLevel = GetHeadingLevel(sLine);

            if (iHeadingLevel > 0)
            {
                FlushParagraph(stringBuilderHtml, stringBuilderParagraph);
                AppendHeading(stringBuilderHtml, listTableOfContentsItems, dictionaryAnchorCounts, iHeadingLevel, sLine);
                continue;
            }

            if (TryParseImage(sLine, out string sImageAlt, out string sImageUrl))
            {
                FlushParagraph(stringBuilderHtml, stringBuilderParagraph);
                AppendImage(stringBuilderHtml, sImageAlt, sImageUrl);
                continue;
            }

            if (stringBuilderParagraph.Length > 0)
            {
                stringBuilderParagraph.Append(' ');
            }

            stringBuilderParagraph.Append(sLine.Trim());
        }

        if (bInsideCodeBlock)
        {
            AppendCodeBlock(stringBuilderHtml, sCodeLanguage, stringBuilderCode.ToString());
        }

        FlushParagraph(stringBuilderHtml, stringBuilderParagraph);

        return new RenderedArticle
        {
            HtmlContent = stringBuilderHtml.ToString(),
            TableOfContentsItems = listTableOfContentsItems
        };
    }

    private static int GetHeadingLevel(string sLine)
    {
        int iHeadingLevel = 0;
        int iMaxLevel = Math.Min(6, sLine.Length);

        for (int iIndex = 0; iIndex < iMaxLevel; iIndex++)
        {
            if (sLine[iIndex] != '#')
            {
                break;
            }

            iHeadingLevel++;
        }

        if (iHeadingLevel == 0 || iHeadingLevel >= sLine.Length || sLine[iHeadingLevel] != ' ')
        {
            return 0;
        }

        return iHeadingLevel;
    }

    private void AppendHeading(
        StringBuilder stringBuilderHtml,
        List<TableOfContentsItem> listTableOfContentsItems,
        Dictionary<string, int> dictionaryAnchorCounts,
        int iHeadingLevel,
        string sLine)
    {
        string sHeadingText = sLine[iHeadingLevel..].Trim();
        string sAnchor = CreateUniqueAnchor(sHeadingText, dictionaryAnchorCounts);
        string sEncodedHeadingText = HtmlEncoder.Default.Encode(sHeadingText);

        if (iHeadingLevel >= 2 && iHeadingLevel <= 4)
        {
            listTableOfContentsItems.Add(new TableOfContentsItem
            {
                Level = iHeadingLevel,
                Text = sHeadingText,
                Anchor = sAnchor
            });
        }

        stringBuilderHtml
            .Append("<h")
            .Append(iHeadingLevel)
            .Append(" id=\"")
            .Append(sAnchor)
            .Append("\">")
            .Append(sEncodedHeadingText)
            .Append("</h")
            .Append(iHeadingLevel)
            .AppendLine(">");
    }

    private string CreateUniqueAnchor(string sHeadingText, Dictionary<string, int> dictionaryAnchorCounts)
    {
        string sAnchor = slugService.CreateSlug(sHeadingText);

        if (!dictionaryAnchorCounts.TryGetValue(sAnchor, out int iAnchorCount))
        {
            dictionaryAnchorCounts[sAnchor] = 1;
            return sAnchor;
        }

        int iNextAnchorCount = iAnchorCount + 1;
        dictionaryAnchorCounts[sAnchor] = iNextAnchorCount;

        return $"{sAnchor}-{iNextAnchorCount}";
    }

    private static void FlushParagraph(StringBuilder stringBuilderHtml, StringBuilder stringBuilderParagraph)
    {
        if (stringBuilderParagraph.Length == 0)
        {
            return;
        }

        stringBuilderHtml
            .Append("<p>")
            .Append(RenderInlineMarkup(stringBuilderParagraph.ToString()))
            .AppendLine("</p>");

        stringBuilderParagraph.Clear();
    }

    private static void AppendCodeBlock(StringBuilder stringBuilderHtml, string sCodeLanguage, string sCode)
    {
        string sLanguage = string.IsNullOrWhiteSpace(sCodeLanguage) ? "text" : sCodeLanguage.Trim();
        string sHighlightedCode = HighlightCode(sCode.TrimEnd('\r', '\n'), sLanguage);

        stringBuilderHtml
            .Append("<div class=\"code-shell\"><div class=\"code-title\"><span>")
            .Append(HtmlEncoder.Default.Encode(sLanguage))
            .Append("</span><button type=\"button\" class=\"code-copy-button\" data-copy-code>Copy</button></div><pre><code class=\"language-")
            .Append(HtmlEncoder.Default.Encode(sLanguage))
            .Append("\">")
            .Append(sHighlightedCode)
            .AppendLine("</code></pre></div>");
    }

    private static string HighlightCode(string sCode, string sLanguage)
    {
        string sEncodedCode = HtmlEncoder.Default.Encode(sCode);

        if (!IsCSharpLanguage(sLanguage))
        {
            return sEncodedCode;
        }

        string[] arrKeywords =
        [
            "public", "private", "protected", "internal", "class", "record", "struct", "interface",
            "void", "string", "int", "double", "decimal", "bool", "char", "var", "new", "return",
            "if", "else", "for", "while", "foreach", "using", "namespace", "async", "await", "true",
            "false", "null", "static", "readonly", "required", "get", "set"
        ];

        for (int iIndex = 0; iIndex < arrKeywords.Length; iIndex++)
        {
            string sKeyword = arrKeywords[iIndex];
            sEncodedCode = Regex.Replace(
                sEncodedCode,
                $@"(?<![\w]){Regex.Escape(sKeyword)}(?![\w])",
                "<span class=\"code-keyword\">$0</span>");
        }

        return sEncodedCode;
    }

    private static bool IsCSharpLanguage(string sLanguage)
    {
        string sLowerLanguage = sLanguage.Trim().ToLowerInvariant();

        return sLowerLanguage is "csharp" or "cs" or "c#";
    }

    private static string RenderInlineMarkup(string sText)
    {
        string sEncodedText = HtmlEncoder.Default.Encode(sText);

        sEncodedText = InlineCodeRegex().Replace(sEncodedText, "<code>$1</code>");
        sEncodedText = BoldRegex().Replace(sEncodedText, "<strong>$1</strong>");
        sEncodedText = LinkRegex().Replace(sEncodedText, static match =>
        {
            string sLinkText = match.Groups["text"].Value;
            string sLinkUrl = HtmlEncoder.Default.Encode(match.Groups["url"].Value);

            return $"<a href=\"{sLinkUrl}\" target=\"_blank\" rel=\"noopener noreferrer\">{sLinkText}</a>";
        });

        return sEncodedText;
    }

    private static bool TryParseImage(string sLine, out string sImageAlt, out string sImageUrl)
    {
        Match matchImage = ImageRegex().Match(sLine.Trim());

        if (!matchImage.Success)
        {
            sImageAlt = "";
            sImageUrl = "";
            return false;
        }

        sImageAlt = matchImage.Groups["alt"].Value;
        sImageUrl = matchImage.Groups["url"].Value;
        return true;
    }

    private static void AppendImage(StringBuilder stringBuilderHtml, string sImageAlt, string sImageUrl)
    {
        stringBuilderHtml
            .Append("<figure class=\"article-image\"><button type=\"button\" data-zoom-image=\"")
            .Append(HtmlEncoder.Default.Encode(sImageUrl))
            .Append("\" aria-label=\"Open image\"><img src=\"")
            .Append(HtmlEncoder.Default.Encode(sImageUrl))
            .Append("\" alt=\"")
            .Append(HtmlEncoder.Default.Encode(sImageAlt))
            .Append("\" loading=\"lazy\"></button><figcaption>")
            .Append(HtmlEncoder.Default.Encode(sImageAlt))
            .AppendLine("</figcaption></figure>");
    }

    [GeneratedRegex("`([^`]+)`")]
    private static partial Regex InlineCodeRegex();

    [GeneratedRegex(@"\*\*([^*]+)\*\*")]
    private static partial Regex BoldRegex();

    [GeneratedRegex(@"\[(?<text>[^\]]+)\]\((?<url>[^)]+)\)")]
    private static partial Regex LinkRegex();

    [GeneratedRegex(@"^!\[(?<alt>[^\]]*)\]\((?<url>[^)]+)\)$")]
    private static partial Regex ImageRegex();
}
