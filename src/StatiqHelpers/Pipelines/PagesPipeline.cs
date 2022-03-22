using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.Extensions;
using StatiqHelpers.Modules;

namespace StatiqHelpers.Pipelines
{
    public class PagesPipeline : Pipeline
    {
        public PagesPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("pages/**/*.{md,mdx,cshtml}")
            };

            ProcessModules = new ModuleList
            {
                new CacheDocuments
                {
                    new ExtractFrontMatter(new ParseYaml()),
                    new GeneratePageDetailsFromPath(),
                    new ExecuteIf(
                        Config.FromDocument(doc => doc.Source.MediaType == MediaTypes.Markdown || doc.Source.MediaType == "text/x-mdx"),
                        new ReplaceInContent(
                            @"!\[(?<alt>.*)\]\(./images/(?<imagePath>.*)\)",
                            Config.FromDocument((document, context) => "![$1](./$2)")).IsRegex(),
                        new ReplaceInContent(
                                @"!\[(?<alt>.*)\]\(./(?<imagePath>.*)\)",
                                Config.FromDocument(
                                    (document, context) => $"![$1](/{Constants.PagesImagesDirectory}/{document.GetString(MetaDataKeys.Slug)}/$2)"))
                            .IsRegex(),
                        new RenderMarkdown().UseExtensions(),
                        new ProcessShortcodes()),
                    new OptimizeSlug(),
                    new SetDestination(Config.FromDocument((doc, ctx) => new NormalizedPath($"{doc.GetString(MetaDataKeys.Slug)}.html")))
                }
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithBaseModel()
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
