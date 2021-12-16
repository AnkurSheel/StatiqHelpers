using Statiq.Common;
using Statiq.Core;

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

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
