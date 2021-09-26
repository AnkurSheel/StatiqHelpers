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

        public static string GetTwitterUserName(this IExecutionContext context)
            => context.GetString("TwitterUsername");
    }
}
