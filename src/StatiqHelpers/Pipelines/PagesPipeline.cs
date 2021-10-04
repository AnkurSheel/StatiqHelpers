using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Pipelines
{
    public class PagesPipeline : Pipeline
    {
        public PagesPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("pages/**/*.{md,cshtml}")
            };

            ProcessModules = new ModuleList
            {
                new CacheDocuments
                {
                    new ExtractFrontMatter(new ParseYaml()),
                    new ExecuteIf(Config.FromDocument(doc => doc.Source.MediaType == MediaTypes.Markdown), new RenderMarkdown().UseExtensions()),
                    new OptimizeFileName(),
                    new SetDestination(".html")
                }
            };

            PostProcessModules = new ModuleList
            {
                new ExecuteIf(Config.FromDocument(doc => doc.Source.MediaType == MediaTypes.Markdown), new RenderRazor().WithBaseModel()).ElseIf(
                    Config.FromDocument(doc => doc.Source.MediaType == MediaTypes.Razor),
                    new RenderRazor().WithModel(
                        Config.FromDocument(
                            (document, context) =>
                            {
                                var posts = context.Outputs.FromPipeline(nameof(PostPipeline)).ToList();
                                return document.AsPagesModel(context, posts);
                            })))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
