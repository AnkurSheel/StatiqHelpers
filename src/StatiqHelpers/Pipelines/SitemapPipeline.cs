using System;
using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Web;

namespace StatiqHelpers.Pipelines
{
    public class SitemapPipeline : Pipeline
    {
        public SitemapPipeline()
        {
            Dependencies.AddRange(nameof(HomePipeline), nameof(PagesPipeline), nameof(PostPipeline));
            ProcessModules = new ModuleList
            {
                new ConcatDocuments(Dependencies.ToArray()),
                new SetMetadata(
                    Keys.SitemapItem,
                    Config.FromDocument(
                        (document, context) =>
                        {
                            var sitemapItem = new SitemapItem(document.GetLink(true));

                            if (document.ContainsKey(MetaDataKeys.PublishedDate))
                            {
                                var originalDate = document.GetDateTime(MetaDataKeys.PublishedDate);
                                var publishedDate = document.GetPublishedDate();

                                if (originalDate.Date <= publishedDate.Date)
                                {
                                    sitemapItem.LastModUtc = DateTime.SpecifyKind(publishedDate, DateTimeKind.Utc);
                                }
                            }

                            return sitemapItem;
                        })),
                new GenerateSitemap()
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
