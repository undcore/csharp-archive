using System.ComponentModel.DataAnnotations;

namespace BlazorWeb.Data;

public class BlogCategory
{
    public int BlogCategoryId { get; set; }

    [MaxLength(80)]
    public string Name { get; set; } = "";

    [MaxLength(120)]
    public string Slug { get; set; } = "";

    [MaxLength(240)]
    public string Description { get; set; } = "";

    public int SortOrder { get; set; }

    public int? ParentBlogCategoryId { get; set; }

    public BlogCategory? ParentBlogCategory { get; set; }

    public ICollection<BlogCategory> ChildCategories { get; set; } = new List<BlogCategory>();

    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
