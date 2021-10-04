using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.Extensions;
using StatiqHelpers.PostDetailsFromPathModule;

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
                    new GeneratePageDetailsFromPath(),
                    new ExecuteIf(
                        Config.FromDocument(doc => doc.Source.MediaType == MediaTypes.Markdown),
                        new ReplaceInContent(
                            @"/images/",
                            "/"),
                        new ReplaceInContent(
                                @"!\[(?<alt>.*)\]\(./(?<imagePath>.*)\)",
                                Config.FromDocument(
                                    (document, context) => $"![$1](/{Constants.PagesImagesDirectory}/{document.GetString(MetaDataKeys.Slug)}/$2)"))
                            .IsRegex(),
                        new RenderMarkdown().UseExtensions()),
                    new OptimizeFileName(MetaDataKeys.Slug),
                    new SetDestination(Config.FromDocument((doc, ctx) => new NormalizedPath($"{doc.GetString(MetaDataKeys.Slug)}.html")))
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
