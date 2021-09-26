using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Feeds;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Pipelines
{
    public class RssPipeline : Pipeline
    {
        public RssPipeline()
        {
            Dependencies.AddRange(nameof(PostPipeline));
            ProcessModules = new ModuleList
            {
                new ConcatDocuments(Dependencies.ToArray()),
                new OrderDocuments(Config.FromDocument(doc => doc.GetPublishedDate())).Descending(),
                new GenerateFeeds().WithRssPath("rss.xml").WithAtomPath(null)
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
