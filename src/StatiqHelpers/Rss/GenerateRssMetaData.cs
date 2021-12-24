using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Statiq.Common;
using Statiq.Feeds;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Rss
{
    public class GenerateRssMetaData : ParallelModule
    {
        protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            context.LogDebug($"Read file {input.Source}");

            return Task.FromResult(
                input.Clone(
                        new MetadataItems
                        {
                            { FeedKeys.Description, input.GetExcerpt() },
                            { FeedKeys.Published, input.GetLastUpdatedDate() },
                            { FeedKeys.Image, input.GetCoverImageLink() }
                        })
                    .Yield());
        }
    }
}
