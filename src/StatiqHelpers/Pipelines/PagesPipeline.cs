using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.CustomExtensions;
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
                        new ReplaceImageLinks(Constants.PagesImagesDirectory),
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
