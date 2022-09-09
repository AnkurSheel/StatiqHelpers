using Statiq.Common;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Modules.RelatedPosts;

public class RelatedPostsPointCalculator : IRelatedPostsPointCalculator
{
    private const int MatchingCategoryPoints = 2;

    public int GetPoints(IDocument input, IDocument document)
    {
        var points = 0;

        if (string.Equals(input.GetCategory(), document.GetCategory(), StringComparison.OrdinalIgnoreCase))
        {
            points += MatchingCategoryPoints;
        }

        var tags = document.GetTags();
        var intersectingTagsCount = tags.Intersect(input.GetTags()).Count();

        points += intersectingTagsCount;
        return points;
    }
}
