using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Modules.RelatedPosts;

public class RelatedPostsService : IRelatedPostsService
{
    private readonly IRelatedPostsPointCalculator _relatedPostsPointCalculator;

    public RelatedPostsService(IRelatedPostsPointCalculator relatedPostsPointCalculator)
    {
        _relatedPostsPointCalculator = relatedPostsPointCalculator;
    }

    public IReadOnlyList<RelatedPostInformation> GetRelatedPosts(IDocument input, IReadOnlyList<IDocument> documents, int numberOfRelatedPosts)
    {
        var postRanks = new HashSet<PostRank>();

        foreach (var document in documents)
        {
            if (input.Source == document.Source)
            {
                continue;
            }

            var points = _relatedPostsPointCalculator.GetPoints(input, document);

            var relativeDate = Math.Abs(input.GetLastUpdatedDate().Subtract(document.GetLastUpdatedDate()).Days);
            postRanks.Add(new PostRank(document, points, relativeDate));
        }

        return postRanks.Where(x => x.Points > 0)
            .OrderByDescending(x => x.Points)
            .ThenBy(x => x.RelativeDate)
            .Take(numberOfRelatedPosts)
            .Select(x => new RelatedPostInformation(x.Document.GetTitle(), x.Document.GetSlug()))
            .ToList();
    }
}
