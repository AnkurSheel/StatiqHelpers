using Microsoft.Extensions.Logging;
using Statiq.Feeds;
using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Modules
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
                            { FeedKeys.Published, input.GetPublishedDate() },
                            { FeedKeys.Updated, input.GetLastUpdatedDate() },
                            { FeedKeys.Image, GetImageLinkForFeeds(input) }
                        })
                    .Yield());
        }

        private string? GetImageLinkForFeeds(IDocument document)
        {
            var coverImagePath = document.GetCoverImagePath();

            if (coverImagePath != null)
            {
                return coverImagePath.StartsWith(Constants.ImagesDirectory)
                    ? coverImagePath
                    : $"/{Constants.SocialImagesDirectory}/{document.GetSlug()}-facebook.png";
            }

            return null;
        }
    }
}
