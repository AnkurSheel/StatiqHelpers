using Statiq.Common;
using Statiq.Core;

namespace StatiqHelpers.Pipelines
{
    public class DownloadsPipeline : Pipeline
    {
        public DownloadsPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("assets/**/*.{pdf,zip,rar,exe}")
            };

            ProcessModules = new ModuleList
            {
                new SetDestination(Config.FromDocument(document => new NormalizedPath($"{Constants.DownloadsDirectory}/{document.Source.FileName}"))),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
