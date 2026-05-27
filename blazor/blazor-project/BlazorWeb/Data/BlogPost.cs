using System.ComponentModel.DataAnnotations;

namespace BlazorWeb.Data;

public class BlogPost
{
    public int BlogPostId { get; set; }

    [MaxLength(180)]
    public string Title { get; set; } = "";

    [MaxLength(220)]
    public string Slug { get; set; } = "";

    [MaxLength(360)]
    public string Summary { get; set; } = "";

    public string ContentMarkdown { get; set; } = "";

    [MaxLength(320)]
    public string? CoverImageUrl { get; set; }

    public bool IsPublished { get; set; }

    public bool IsFeatured { get; set; }

    public int ViewCount { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAtUtc { get; set; }

    public int BlogCategoryId { get; set; }

    public BlogCategory? BlogCategory { get; set; }

    public string AuthorId { get; set; } = "";

    public ApplicationUser? Author { get; set; }

    public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
}
