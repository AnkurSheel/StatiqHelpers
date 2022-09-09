using Microsoft.Extensions.Logging;
using Statiq.Common;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Modules.RelatedPosts;

public class GenerateRelatedPosts : ParallelModule
{
    private readonly IRelatedPostsService _relatedPostsService;

    public GenerateRelatedPosts(IRelatedPostsService relatedPostsService)
    {
        _relatedPostsService = relatedPostsService;
    }

    protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
    {
        context.LogDebug("Read file {InputSource}", input.Source);

        var documents = context.Outputs.FromPipeline(nameof(PostPipeline)).ToList();

        var numberOfRelatedPosts = context.GetInt("NumberOfRelatedPosts", 4);

        var sortedRelatedPosts = _relatedPostsService.GetRelatedPosts(input, documents, numberOfRelatedPosts);

        return Task.FromResult(
            input.Clone(
                    new MetadataItems
                    {
                        { Keys.RelatedPosts, sortedRelatedPosts }
                    })
                .Yield());
    }
}
