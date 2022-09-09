using Statiq.Common;

namespace StatiqHelpers.Modules.RelatedPosts;

public interface IRelatedPostsService
{
    IReadOnlyList<RelatedPostInformation> GetRelatedPosts(IDocument input, IReadOnlyList<IDocument> documents, int numberOfRelatedPosts);
}

public record PostRank(IDocument Document, int Points, int RelativeDate);

public record RelatedPostInformation(string Title, string Path);
