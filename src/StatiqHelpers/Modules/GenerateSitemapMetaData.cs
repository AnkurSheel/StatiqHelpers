using Microsoft.Extensions.Logging;
using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Modules;

public class GenerateSitemapMetaData : ParallelModule
{
    protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
    {
        context.LogDebug($"Read file {input.Source}");

        var sitemapItem = new SitemapItem(input.GetLink(true));

        if (input.ContainsKey(MetaDataKeys.PublishedDate))
        {
            var publishedDate = input.GetDateTime(MetaDataKeys.PublishedDate);
            var lastUpdatedDate = input.GetLastUpdatedDate();

            var modifiedDate = lastUpdatedDate.Date >= publishedDate.Date
                ? lastUpdatedDate
                : publishedDate;

            sitemapItem.LastModUtc = DateTime.SpecifyKind(modifiedDate, DateTimeKind.Utc);
        }

        return Task.FromResult(
            input.Clone(
                    new MetadataItems
                    {
                        { Keys.SitemapItem, sitemapItem },
                    })
                .Yield());
    }
}
