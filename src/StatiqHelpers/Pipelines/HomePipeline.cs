using Statiq.Razor;
using StatiqHelpers.CustomExtensions;

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
                new SetMetadata(MetaDataKeys.Excerpt, Config.FromContext(context => context.GetDescription())),
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
