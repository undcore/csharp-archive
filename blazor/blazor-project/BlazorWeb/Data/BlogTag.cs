using System.ComponentModel.DataAnnotations;

namespace BlazorWeb.Data;

public class BlogTag
{
    public int BlogTagId { get; set; }

    [MaxLength(80)]
    public string Name { get; set; } = "";

    [MaxLength(120)]
    public string Slug { get; set; } = "";

    public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
}
