using Statiq.Common;
using Statiq.Core;
using Statiq.Minification;

namespace StatiqHelpers.Pipelines
{
    public class CssPipeline : Pipeline
    {
        public CssPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("assets/**/{*,!_*}.css")
            };

            ProcessModules = new ModuleList
            {
                new MinifyCss(),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
