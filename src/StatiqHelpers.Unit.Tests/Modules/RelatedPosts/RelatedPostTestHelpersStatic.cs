using Statiq.Testing;
using StatiqHelpers.CustomExtensions;
using StatiqHelpers.Modules.RelatedPosts;
using Keys = Statiq.Common.Keys;

namespace StatiqHelpers.Unit.Tests.Modules.RelatedPosts;

public static class RelatedPostTestHelpersStatic
{
    public static RelatedPostInformation GetRelatedPostInformation(IDocument document)
        => new RelatedPostInformation(document.GetTitle(), document.GetSlug());

    public static TestDocument GetTestDocument(string path, string title, string slug)
        => new TestDocument(new NormalizedPath(path))
        {
            { Keys.Title, title },
            { MetaDataKeys.Slug, slug },
        };
}
