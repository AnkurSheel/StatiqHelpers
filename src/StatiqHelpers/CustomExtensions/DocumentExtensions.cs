using System.Text.RegularExpressions;
using StatiqHelpers.Models;
using StatiqHelpers.Modules.ReadingTime;
using StatiqHelpers.Modules.RelatedPosts;
using StatiqHelpers.Pipelines;
using Keys = StatiqHelpers.Modules.ReadingTime.Keys;

namespace StatiqHelpers.CustomExtensions;

public static class DocumentExtensions
{
    public static GroupCollection GetPostDetailsFromPath(this IDocument doc)
    {
        var regex = new Regex(@".*(?<year>[\d]{4})-(?<month>[\d]{2})-(?<date>[\d]{2})-(?<slug>.+)$");
        var m = regex.Match(doc.Source.Parent.ToString());
        return m.Groups;
    }

    public static string GetExcerpt(this IDocument document)
    {
        return document.GetString(MetaDataKeys.Excerpt);
    }

    public static string GetSlug(this IDocument document)
    {
        return document.GetString(MetaDataKeys.Slug);
    }

    public static DateTime GetPublishedDate(this IDocument document)
    {
        return DateTime.SpecifyKind(document.GetDateTime(MetaDataKeys.PublishedDate), DateTimeKind.Utc);
    }

    public static DateTime GetLastUpdatedDate(this IDocument document)
    {
        return DateTime.SpecifyKind(document.GetDateTime(MetaDataKeys.UpdatedOnDate, document.GetPublishedDate()),
            DateTimeKind.Utc);
    }

    public static string? GetCoverImagePath(this IDocument document)
    {
        var coverImage = document.GetString(MetaDataKeys.CoverImage);
        return coverImage?.TrimStart('.', '/');
    }

    public static string GetPageUrl(this IDocument document, bool relative = true)
    {
        return document.GetLink(!relative);
    }

    public static string? GetCoverImageLink(this IDocument document)
    {
        var coverImagePath = document.GetCoverImagePath();

        if (coverImagePath != null)
        {
            return coverImagePath.StartsWith(Constants.ImagesDirectory)
                ? coverImagePath
                : $"/{Constants.PostImagesDirectory}/{document.GetSlug()}/{coverImagePath}";
        }

        return null;
    }

    public static IReadOnlyList<string> GetTags(this IDocument document)
    {
        return document.GetList<string>(MetaDataKeys.Tags);
    }

    public static string GetCategory(this IDocument document)
    {
        return document.GetString(MetaDataKeys.Category) ?? Constants.DefaultCategory;
    }

    public static ReadingTimeData GetReadingTime(this IDocument document)
    {
        return document.Get<ReadingTimeData>(Keys.ReadingTime);
    }

    public static IReadOnlyList<RelatedPostInformation> GetRelatedPosts(this IDocument document)
    {
        return document.GetList<RelatedPostInformation>(Modules.RelatedPosts.Keys.RelatedPosts);
    }

    public static string? GetImageFacebook(this IDocument document)
    {
        var slug = document.GetSlug();
        return !string.IsNullOrWhiteSpace(slug)
            ? IExecutionContext.Current.GetLink($"{Constants.SocialImagesDirectory}/{slug}-facebook.png", true)
            : null;
    }

    public static string? GetImageTwitter(this IDocument document)
    {
        var slug = document.GetSlug();
        return !string.IsNullOrWhiteSpace(slug)
            ? IExecutionContext.Current.GetLink($"{Constants.SocialImagesDirectory}/{slug}-twitter.png", true)
            : null;
    }

    public static PageModel AsPagesModel(this IDocument document, IExecutionContext context,
        IReadOnlyList<IDocument> posts)
    {
        return new PageModel(document, context, posts);
    }

    public static BaseModel AsBaseModel(this IDocument document, IExecutionContext context)
    {
        return new BaseModel(document, context);
    }

    public static Tag AsTag(this IDocument document, PipelineOptions options, IExecutionContext context)
    {
        var posts = options.OrderFunction(document.GetChildren()).Select(x => x.AsBaseModel(context)).ToList();

        var name = document.GetString(MetaDataKeys.TagName);
        return new Tag(document, context, name, new NormalizedPath($"/tags/{name}").OptimizeFileName().ToString(),
            posts);
    }
}
