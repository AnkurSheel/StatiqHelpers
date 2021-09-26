using Statiq.Common;
using Statiq.Core;

namespace StatiqHelpers.Pipelines
{
    public class FontsPipeline : Pipeline
    {
        public FontsPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("assets/**/*.ttf")
            };

            ProcessModules = new ModuleList
            {
                new SetDestination(Config.FromDocument(document => new NormalizedPath($"{Constants.FontsDirectory}/{document.Source.FileName}"))),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
