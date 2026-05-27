using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorWeb.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();

    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    public DbSet<BlogTag> BlogTags => Set<BlogTag>();

    public DbSet<BlogPostTag> BlogPostTags => Set<BlogPostTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .Property(applicationUser => applicationUser.Nickname)
            .HasMaxLength(80);

        modelBuilder.Entity<BlogCategory>()
            .HasIndex(blogCategory => blogCategory.Slug)
            .IsUnique();

        modelBuilder.Entity<BlogCategory>()
            .HasOne(blogCategory => blogCategory.ParentBlogCategory)
            .WithMany(blogCategory => blogCategory.ChildCategories)
            .HasForeignKey(blogCategory => blogCategory.ParentBlogCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BlogPost>()
            .HasIndex(blogPost => blogPost.Slug)
            .IsUnique();

        modelBuilder.Entity<BlogPost>()
            .HasOne(blogPost => blogPost.BlogCategory)
            .WithMany(blogCategory => blogCategory.BlogPosts)
            .HasForeignKey(blogPost => blogPost.BlogCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BlogPost>()
            .HasOne(blogPost => blogPost.Author)
            .WithMany(applicationUser => applicationUser.BlogPosts)
            .HasForeignKey(blogPost => blogPost.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BlogTag>()
            .HasIndex(blogTag => blogTag.Slug)
            .IsUnique();

        modelBuilder.Entity<BlogPostTag>()
            .HasKey(blogPostTag => new { blogPostTag.BlogPostId, blogPostTag.BlogTagId });

        modelBuilder.Entity<BlogPostTag>()
            .HasOne(blogPostTag => blogPostTag.BlogPost)
            .WithMany(blogPost => blogPost.BlogPostTags)
            .HasForeignKey(blogPostTag => blogPostTag.BlogPostId);

        modelBuilder.Entity<BlogPostTag>()
            .HasOne(blogPostTag => blogPostTag.BlogTag)
            .WithMany(blogTag => blogTag.BlogPostTags)
            .HasForeignKey(blogPostTag => blogPostTag.BlogTagId);
    }
}
