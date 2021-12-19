using System.Collections.Generic;
using System.Linq;
using Statiq.Common;
using Statiq.Web;
using StatiqHelpers.Models;

namespace StatiqHelpers.Extensions
{
    public static class ContextExtensions
    {
        public static string GetSiteTitle(this IExecutionContext context)
            => context.GetString(Keys.Title);

        public static IReadOnlyList<NavigationLink> GetNavigationLinks(this IExecutionContext context)
            => context.GetDocumentList("HeaderLinks").Select(x => new NavigationLink(x.GetString("Title"), x.GetString("Url"))).ToList();

        public static string GetScript(this IExecutionContext context)
            => context.GetLink($"/{Constants.JsDirectory}/blog.js");

        public static string GetDescription(this IExecutionContext context)
            => context.GetString(WebKeys.Description);

        public static string GetCanonicalUrl(this IExecutionContext context, IDocument document)
            => context.GetString("canonicalUrl") ?? document.GetPageUrl(false);

        public static string GetTwitterUsername(this IExecutionContext context)
            => context.GetString("TwitterUsername");

        public static string GetLinkedInUsername(this IExecutionContext context)
            => context.GetString("LinkedInUsername");

        public static string GetBuyMeACoffeeUsername(this IExecutionContext context)
            => context.GetString("BuyMeACoffeeUsername");

        public static string GetGithubUserName(this IExecutionContext context)
            => context.GetString("GithubUsername");

        public static string? GetGoogleTagManagerId(this IExecutionContext context)
            => context.IsDevelopment()
                ? context.GetString("GoogleTagManagerId")
                : null;

        public static bool IsDevelopment(this IExecutionContext context)
            => context.GetString("Environment") != "Development";

        public static string? GetGoatCounterCode(this IExecutionContext context)
            => context.GetString("GoatCounterCode");
    }
}
