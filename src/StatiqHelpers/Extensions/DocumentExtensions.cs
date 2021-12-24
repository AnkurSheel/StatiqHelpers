using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Statiq.Common;
using StatiqHelpers.Models;
using StatiqHelpers.Modules.ReadingTime;
using Keys = StatiqHelpers.Modules.ReadingTime.Keys;

namespace StatiqHelpers.Extensions
{
    public static class DocumentExtensions
    {
        public static GroupCollection GetPostDetailsFromPath(this IDocument doc)
        {
            var regex = new Regex(@".*(?<year>[\d]{4})-(?<month>[\d]{2})-(?<date>[\d]{2})-(?<slug>.+)$");
            var m = regex.Match(doc.Source.Parent.ToString());
            return m.Groups;
        }

        public static string GetExcerpt(this IDocument document)
            => document.GetString(MetaDataKeys.Excerpt);

        public static string GetSlug(this IDocument document)
            => document.GetString(MetaDataKeys.Slug);

        public static DateTime GetPublishedDate(this IDocument document)
            => document.GetDateTime(MetaDataKeys.PublishedDate);

        public static DateTime GetLastUpdatedDate(this IDocument document)
            => document.GetDateTime(MetaDataKeys.UpdatedOnDate, document.GetPublishedDate());

        public static string? GetCoverImagePath(this IDocument document)
        {
            var coverImage = document.GetString(MetaDataKeys.CoverImage);
            return coverImage?.TrimStart('.', '/');
        }

        public static string GetPageUrl(this IDocument document, bool relative = true)
            => document.GetLink(!relative);

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
            => document.GetList<string>(MetaDataKeys.Tags);

        public static string GetCategory(this IDocument document)
            => document.GetString(MetaDataKeys.Category) ?? "Uncategorized";

        public static ReadingTimeData GetReadingTime(this IDocument document)
            => document.Get<ReadingTimeData>(Keys.ReadingTime);

        public static string GetImageFacebook(this IDocument document)
            => IExecutionContext.Current.GetLink($"{Constants.SocialImagesDirectory}/{document.GetSlug()}-facebook.png", true);

        public static string GetImageTwitter(this IDocument document)
            => IExecutionContext.Current.GetLink($"{Constants.SocialImagesDirectory}/{document.GetSlug()}-twitter.png", true);

        public static PageModel AsPagesModel(this IDocument document, IExecutionContext context, IReadOnlyList<IDocument> posts)
            => new PageModel(document, context, posts);

        public static BaseModel AsBaseModel(this IDocument document, IExecutionContext context)
            => new BaseModel(document, context);

        public static Tag AsTag(this IDocument document, IExecutionContext context)
        {
            var posts = document.GetChildren().OrderByDescending(x => x.GetLastUpdatedDate()).Select(x => x.AsBaseModel(context)).ToList();

            var name = document.GetString(MetaDataKeys.TagName);
            return new Tag(document, context, name, new NormalizedPath($"/tags/{name}").OptimizeFileName().ToString(), posts);
        }
    }
}
