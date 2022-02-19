using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Pipelines
{
    public class HomePipeline : Pipeline
    {
        public HomePipeline()
        {
            Dependencies.AddRange(nameof(PostPipeline));
            InputModules = new ModuleList
            {
                new ReadFiles("Index.cshtml")
            };

            ProcessModules = new ModuleList
            {
                new OptimizeFileName(),
                new SetDestination(".html"),
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithModel(
                    Config.FromDocument(
                        (document, context) =>
                        {
                            var posts = context.Outputs.FromPipeline(nameof(PostPipeline)).ToList();
                            return document.AsPagesModel(context, posts);
                        }))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
