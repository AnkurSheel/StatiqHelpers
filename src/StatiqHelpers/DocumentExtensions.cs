using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Statiq.Common;
using StatiqHelpers.ReadingTimeModule;
using Keys = StatiqHelpers.ReadingTimeModule.Keys;

namespace StatiqHelpers
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

        public static DateTime GetUpdatedTime(this IDocument document)
            => document.GetPublishedDate();

        public static string GetCoverImagePath(this IDocument document)
            => document.GetString(MetaDataKeys.CoverImage).TrimStart('.', '/');

        public static string GetPageUrl(this IDocument document, bool relative = true)
            => document.GetLink(!relative);

        public static string GetCoverImageLink(this IDocument document)
            => $"/{Constants.PostImagesDirectory}/{document.GetSlug()}/{document.GetCoverImagePath()}";

        public static IReadOnlyList<string> GetTags(this IDocument document)
            => document.GetList<string>(MetaDataKeys.Tags);

        public static ReadingTimeData GetReadingTime(this IDocument document)
            => document.Get<ReadingTimeData>(Keys.ReadingTime);

        public static string GetImageFacebook(this IDocument document)
            => IExecutionContext.Current.GetLink($"{Constants.SocialImagesDirectory}/{document.GetSlug()}-facebook.png", true);

        public static string GetImageTwitter(this IDocument document)
            => IExecutionContext.Current.GetLink($"{Constants.SocialImagesDirectory}/{document.GetSlug()}-twitter.png", true);
    }
}
