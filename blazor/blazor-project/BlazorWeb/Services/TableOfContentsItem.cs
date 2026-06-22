namespace BlazorWeb.Services;

public sealed class TableOfContentsItem
{
    public int Level { get; init; }

    public string Text { get; init; } = "";

    public string Anchor { get; init; } = "";
}
