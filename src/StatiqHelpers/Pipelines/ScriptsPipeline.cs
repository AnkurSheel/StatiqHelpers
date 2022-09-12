using Statiq.Minification;

namespace StatiqHelpers.Pipelines
{
    public class ScriptsPipeline : Pipeline
    {
        public ScriptsPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("assets/js/*.js")
            };

            ProcessModules = new ModuleList
            {
                new MinifyJs(),
                new ExecuteIf(Config.FromDocument(doc => doc.Source.Name.Contains("sw.js")))
                {
                    new SetDestination("sw.js")
                }
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
