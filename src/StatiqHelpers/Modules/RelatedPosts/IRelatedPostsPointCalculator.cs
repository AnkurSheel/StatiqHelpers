using Statiq.Common;

namespace StatiqHelpers.Modules.RelatedPosts;

public interface IRelatedPostsPointCalculator
{
    int GetPoints(IDocument input, IDocument document);
}
