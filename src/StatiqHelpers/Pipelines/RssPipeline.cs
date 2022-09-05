using Statiq.Feeds;
using StatiqHelpers.CustomExtensions;

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
                new OrderDocuments(Config.FromDocument(doc => doc.GetLastUpdatedDate())).Descending(),
                new GenerateFeeds().WithRssPath("rss.xml").WithAtomPath(null)
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
