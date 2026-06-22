using BlazorWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorWeb.Data;

public static class BlogDatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        ApplicationDbContext applicationDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        SlugService slugService = serviceProvider.GetRequiredService<SlugService>();

        await applicationDbContext.Database.MigrateAsync();
        await EnsureRolesAsync(roleManager);
        ApplicationUser? applicationUserAdmin = await EnsureAdminUserAsync(userManager, configuration);
        await EnsureCategoriesAsync(applicationDbContext);
        await EnsureSamplePostsAsync(applicationDbContext, slugService, applicationUserAdmin);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] arrRoleNames = [BlogRoles.Admin, BlogRoles.User];

        for (int iIndex = 0; iIndex < arrRoleNames.Length; iIndex++)
        {
            string sRoleName = arrRoleNames[iIndex];

            if (!await roleManager.RoleExistsAsync(sRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(sRoleName));
            }
        }
    }

    private static async Task<ApplicationUser?> EnsureAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        string? sAdminEmail = configuration["SeedAdmin:Email"];
        string? sAdminPassword = configuration["SeedAdmin:Password"];
        string sAdminUserName = configuration["SeedAdmin:UserName"] ?? "admin";
        string sAdminNickname = configuration["SeedAdmin:Nickname"] ?? "Admin";

        if (string.IsNullOrWhiteSpace(sAdminEmail) || string.IsNullOrWhiteSpace(sAdminPassword))
        {
            return null;
        }

        ApplicationUser? applicationUserAdmin = await userManager.FindByEmailAsync(sAdminEmail);

        if (applicationUserAdmin is null)
        {
            applicationUserAdmin = new ApplicationUser
            {
                UserName = sAdminUserName,
                Email = sAdminEmail,
                EmailConfirmed = true,
                Nickname = sAdminNickname
            };

            IdentityResult identityResult = await userManager.CreateAsync(applicationUserAdmin, sAdminPassword);

            if (!identityResult.Succeeded)
            {
                return null;
            }
        }

        if (!await userManager.IsInRoleAsync(applicationUserAdmin, BlogRoles.Admin))
        {
            await userManager.AddToRoleAsync(applicationUserAdmin, BlogRoles.Admin);
        }

        if (!await userManager.IsInRoleAsync(applicationUserAdmin, BlogRoles.User))
        {
            await userManager.AddToRoleAsync(applicationUserAdmin, BlogRoles.User);
        }

        return applicationUserAdmin;
    }

    private static async Task EnsureCategoriesAsync(ApplicationDbContext applicationDbContext)
    {
        List<CategorySeedItem> listCategorySeedItems =
        [
            new("Lang", "lang", "프로그래밍 언어 학습 기록", ["C", "C++", "Java", "C#", "Python"]),
            new("CS", "cs", "컴퓨터공학 전공 지식 정리", ["DataStructure", "Algorithm", "OS", "Network", "ComputerArchitecture", "Security"]),
            new("DB", "db", "데이터베이스 설계와 SQL 학습 기록", ["Database", "Oracle", "MSSQL"]),
            new("Framework", "framework", "프레임워크와 백엔드 개발 기록", ["Spring", "ASP.NET", "Blazor", "JPA"]),
            new("Mobile", "mobile", "모바일 개발 기록", ["Android"]),
            new("Project", "project", "과제, 개인, 팀 프로젝트 개발 일지", ["University", "Personal", "Team"]),
            new("License", "license", "자격증 학습 아카이브", ["정보처리기사", "SQLD", "ADSP", "네트워크관리사"]),
            new("DevLog", "devlog", "개발 과정과 회고 기록", [])
        ];

        for (int iIndex = 0; iIndex < listCategorySeedItems.Count; iIndex++)
        {
            CategorySeedItem categorySeedItem = listCategorySeedItems[iIndex];
            BlogCategory? blogCategoryRoot = await applicationDbContext.BlogCategories
                .FirstOrDefaultAsync(blogCategory => blogCategory.Slug == categorySeedItem.Slug);

            if (blogCategoryRoot is null)
            {
                blogCategoryRoot = new BlogCategory
                {
                    Name = categorySeedItem.Name,
                    Slug = categorySeedItem.Slug,
                    Description = categorySeedItem.Description,
                    SortOrder = iIndex
                };

                applicationDbContext.BlogCategories.Add(blogCategoryRoot);
            }

            for (int iChildIndex = 0; iChildIndex < categorySeedItem.Children.Length; iChildIndex++)
            {
                string sChildName = categorySeedItem.Children[iChildIndex];
                string sChildSlug = CreateChildSlug(categorySeedItem.Slug, sChildName);
                bool bChildExists = await applicationDbContext.BlogCategories
                    .AnyAsync(blogCategory => blogCategory.Slug == sChildSlug);

                if (!bChildExists)
                {
                    applicationDbContext.BlogCategories.Add(new BlogCategory
                    {
                        Name = sChildName,
                        Slug = sChildSlug,
                        Description = $"{categorySeedItem.Name} / {sChildName}",
                        ParentBlogCategory = blogCategoryRoot,
                        SortOrder = iChildIndex
                    });
                }
            }
        }

        await applicationDbContext.SaveChangesAsync();
    }

    private static async Task EnsureSamplePostsAsync(
        ApplicationDbContext applicationDbContext,
        SlugService slugService,
        ApplicationUser? applicationUserAdmin)
    {
        if (applicationUserAdmin is null)
        {
            return;
        }

        if (await applicationDbContext.BlogPosts.AnyAsync())
        {
            await RepairSamplePostSlugsAsync(applicationDbContext, slugService);
            return;
        }

        BlogCategory? blogCategoryBlazor = await applicationDbContext.BlogCategories.FirstOrDefaultAsync(blogCategory => blogCategory.Slug == "framework-blazor");
        BlogCategory? blogCategoryOs = await applicationDbContext.BlogCategories.FirstOrDefaultAsync(blogCategory => blogCategory.Slug == "cs-os");
        BlogCategory? blogCategoryMssql = await applicationDbContext.BlogCategories.FirstOrDefaultAsync(blogCategory => blogCategory.Slug == "db-mssql");

        if (blogCategoryBlazor is null || blogCategoryOs is null || blogCategoryMssql is null)
        {
            return;
        }

        List<BlogPost> listBlogPosts =
        [
            CreatePost(
                applicationUserAdmin,
                blogCategoryBlazor,
                slugService,
                "Blazor Server 학습 기록: 인증 흐름 정리",
                "ASP.NET Core Identity 기반 로그인, 이메일 인증, 관리자 권한 분리를 블로그 구조와 연결한 기록입니다.",
                """
                ## 학습 목표

                Blazor Server에서 Identity를 사용하면 인증 쿠키, 사용자 관리, 토큰 발급을 직접 구현하지 않아도 된다.

                ## 핵심 코드

                ```csharp
                public async Task LoginAsync(string sUserId, string sPassword)
                {
                    ApplicationUser? applicationUser = await userManager.FindByNameAsync(sUserId);

                    if (applicationUser is null)
                    {
                        return;
                    }

                    await signInManager.SignInAsync(applicationUser, isPersistent: false);
                }
                ```

                ## 정리

                관리자 권한은 글 작성 화면 접근 제어에 사용하고, 일반 사용자는 읽기와 계정 관리 중심으로 둔다.
                """,
                ["Blazor", "ASP.NET", "Identity"]),
            CreatePost(
                applicationUserAdmin,
                blogCategoryOs,
                slugService,
                "운영체제 정리: 프로세스와 스레드",
                "프로세스, 스레드, 컨텍스트 스위칭을 과제와 면접 복습에 바로 사용할 수 있게 정리했습니다.",
                """
                ## 프로세스

                프로세스는 실행 중인 프로그램의 인스턴스이며 독립적인 주소 공간을 가진다.

                ## 스레드

                스레드는 프로세스 내부의 실행 단위이며 코드, 데이터, 힙 영역을 공유한다.

                ## 비교

                컨텍스트 스위칭 비용은 일반적으로 프로세스보다 스레드가 작다. 다만 공유 자원 접근에는 동기화 비용이 생긴다.
                """,
                ["OS", "CS", "Thread"]),
            CreatePost(
                applicationUserAdmin,
                blogCategoryMssql,
                slugService,
                "MSSQL 인덱스 설계 노트",
                "블로그 검색과 게시글 목록 성능을 기준으로 클러스터드/논클러스터드 인덱스 사용 기준을 정리했습니다.",
                """
                ## 인덱스가 필요한 지점

                게시글 목록은 공개 여부, 작성일, 카테고리 조건으로 자주 조회된다.

                ## 예시 쿼리

                ```sql
                SELECT Title, Slug, PublishedAtUtc
                FROM BlogPosts
                WHERE IsPublished = 1
                ORDER BY PublishedAtUtc DESC;
                ```

                ## 설계 기준

                읽기 빈도가 높은 정렬 조건과 필터 조건을 기준으로 인덱스를 검토한다.
                """,
                ["MSSQL", "Database", "Index"])
        ];

        for (int iIndex = 0; iIndex < listBlogPosts.Count; iIndex++)
        {
            applicationDbContext.BlogPosts.Add(listBlogPosts[iIndex]);
        }

        await applicationDbContext.SaveChangesAsync();
    }

    private static async Task RepairSamplePostSlugsAsync(
        ApplicationDbContext applicationDbContext,
        SlugService slugService)
    {
        string[] arrSampleTitles =
        [
            "Blazor Server 학습 기록: 인증 흐름 정리",
            "운영체제 정리: 프로세스와 스레드",
            "MSSQL 인덱스 설계 노트"
        ];

        for (int iIndex = 0; iIndex < arrSampleTitles.Length; iIndex++)
        {
            string sSampleTitle = arrSampleTitles[iIndex];
            BlogPost? blogPost = await applicationDbContext.BlogPosts
                .FirstOrDefaultAsync(blogPostItem => blogPostItem.Title == sSampleTitle);

            if (blogPost is null)
            {
                continue;
            }

            string sExpectedSlug = slugService.CreateSlug(sSampleTitle);

            if (blogPost.Slug == sExpectedSlug)
            {
                continue;
            }

            blogPost.Slug = sExpectedSlug;
            blogPost.UpdatedAtUtc = DateTime.UtcNow;
        }

        await applicationDbContext.SaveChangesAsync();
    }

    private static BlogPost CreatePost(
        ApplicationUser applicationUserAdmin,
        BlogCategory blogCategory,
        SlugService slugService,
        string sTitle,
        string sSummary,
        string sContentMarkdown,
        string[] arrTagNames)
    {
        BlogPost blogPost = new()
        {
            Author = applicationUserAdmin,
            BlogCategory = blogCategory,
            Title = sTitle,
            Slug = slugService.CreateSlug(sTitle),
            Summary = sSummary,
            ContentMarkdown = sContentMarkdown,
            IsFeatured = true,
            IsPublished = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            PublishedAtUtc = DateTime.UtcNow
        };

        for (int iIndex = 0; iIndex < arrTagNames.Length; iIndex++)
        {
            string sTagName = arrTagNames[iIndex];
            BlogTag blogTag = new()
            {
                Name = sTagName,
                Slug = slugService.CreateSlug(sTagName)
            };

            blogPost.BlogPostTags.Add(new BlogPostTag
            {
                BlogPost = blogPost,
                BlogTag = blogTag
            });
        }

        return blogPost;
    }

    private static string CreateChildSlug(string sParentSlug, string sChildName)
    {
        string sChildSlug = sChildName.ToLowerInvariant()
            .Replace("#", "sharp", StringComparison.Ordinal)
            .Replace("+", "plus", StringComparison.Ordinal)
            .Replace(".", "", StringComparison.Ordinal)
            .Replace(" ", "-", StringComparison.Ordinal);

        return sChildName switch
        {
            "정보처리기사" => $"{sParentSlug}-information-processing-engineer",
            "네트워크관리사" => $"{sParentSlug}-network-manager",
            _ => $"{sParentSlug}-{sChildSlug}"
        };
    }

    private sealed record CategorySeedItem(string Name, string Slug, string Description, string[] Children);
}
