namespace BlazorWeb.Services;

public sealed class RenderedArticle
{
    public string HtmlContent { get; init; } = "";

    public List<TableOfContentsItem> TableOfContentsItems { get; init; } = [];
}
